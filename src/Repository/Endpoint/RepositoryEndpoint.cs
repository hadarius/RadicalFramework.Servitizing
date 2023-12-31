using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Radical.Series;
using Radical.Servitizing.Configuration;
using Radical.Servitizing.Data.Store;
using Radical.Uniques;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Radical.Servitizing.Repository.Endpoint;

using Radical.Servitizing.Entity;
using Security.Identity;

public class RepositoryEndpoint : Catalog<IRepositoryContext>, IRepositoryEndpoint
{
    protected DbContextConfigurationSnapshot _configurationSnapshot;
    protected Type contextType;
    protected new bool disposedValue;
    protected DbContextOptionsBuilder optionsBuilder;
    protected Uscn servicecode;
    const int WAIT_PUBLISH_TIMEOUT = 30 * 1000;
    ManualResetEventSlim _access = new ManualResetEventSlim(true, 128);
    SemaphoreSlim _pass = new SemaphoreSlim(1);

    public RepositoryEndpoint()
    {
        Site = DataSite.Endpoint;
    }

    public RepositoryEndpoint(DbContextOptions options) : this()
    {
        ContextPool = this;
        InnerContext = CreateContext(options);
        Context.GetEntityTypes();
    }

    public RepositoryEndpoint(IRepositoryContextPool pool) : this()
    {
        PoolSize = pool.PoolSize;
        ContextPool = pool;
        InnerContext = pool.CreateContext();
    }

    public RepositoryEndpoint(EndpointProvider provider, string connectionString) : this()
    {
        ContextPool = this;
        optionsBuilder = RepositoryEndpointOptionsBuilder.BuildOptions(provider, connectionString);
        InnerContext = CreateContext(optionsBuilder.Options);
        Context.GetEntityTypes();
    }

    public RepositoryEndpoint(Type contextType, IServiceConfiguration config) : this()
    {
        IConfigurationSection endpoint = config.Endpoint(contextType.FullName);
        string connectionString = config.EndpointConnectionString(contextType.FullName);
        EndpointProvider provider = config.EndpointProvider(contextType.FullName);
        ContextPool = this;
        PoolSize = config.EndpointPoolSize(endpoint);
        optionsBuilder = RepositoryEndpointOptionsBuilder.BuildOptions(provider, connectionString);
        InnerContext = CreateContext(optionsBuilder.Options);
        Context.GetEntityTypes();
    }

    public RepositoryEndpoint(Type contextType, DbContextOptions options) : this()
    {
        ContextPool = this;
        InnerContext = CreateContext(contextType, options);
        Context.GetEntityTypes();
    }

    public RepositoryEndpoint(
        Type contextType,
        EndpointProvider provider,
        string connectionString
    ) : this()
    {
        ContextPool = this;
        optionsBuilder = RepositoryEndpointOptionsBuilder.BuildOptions(provider, connectionString);
        InnerContext = CreateContext(contextType, optionsBuilder.Options);
        Context.GetEntityTypes();
    }

    public virtual IDataStoreContext Context => (IDataStoreContext)InnerContext;

    public IRepositoryContext ContextLease { get; set; }

    public IRepositoryContextPool ContextPool { get; set; }

    public Type ContextType => contextType ??= InnerContext.GetType();

    public object InnerContext { get; set; }

    public bool Leased => ContextLease != null;

    public virtual DbContextOptions Options { get; protected set; }

    public bool Pooled => ContextPool != null;

    public int PoolSize { get; set; }

    public DataSite Site { get; set; }

    public override ulong Key
    {
        get =>
            servicecode.Key == 0
                ? (servicecode.Key = Unique.NewKey)
                : servicecode.Key;
        set => servicecode.Key = value;
    }

    public override ulong TypeKey
    {
        get =>
            servicecode.TypeKey == 0
                ? (servicecode.TypeKey = ContextType.GetProxyEntityTypeKey32())
                : servicecode.TypeKey;
        set => servicecode.TypeKey = value;
    }

    public void AcquireAccess()
    {
        do
        {
            if (!_access.Wait(WAIT_PUBLISH_TIMEOUT))
            {
                throw new TimeoutException("Wait write Timeout");
            }

            _access.Reset();
        } while (!_pass.Wait(0));
    }

    public virtual object CreateContext()
    {
        return ContextType.New(Options);
    }

    public virtual TContext CreateContext<TContext>() where TContext : class
    {
        contextType ??= typeof(TContext);
        return typeof(TContext).New<TContext>(Options);
    }

    public virtual IDataStoreContext CreateContext(DbContextOptions options)
    {
        contextType ??= options.ContextType;
        Options ??= options;
        return (IDataStoreContext)ContextType.New(options);
    }

    public virtual TContext CreateContext<TContext>(DbContextOptions options)
        where TContext : DbContext
    {
        Options ??= options;
        contextType ??= typeof(TContext);
        return typeof(TContext).New<TContext>(options);
    }

    public virtual IDataStoreContext CreateContext(Type contextType, DbContextOptions options)
    {
        this.contextType ??= contextType;
        Options ??= options;
        return (IDataStoreContext)ContextType.New(options);
    }

    public void CreatePool()
    {
        SnapshotConfiguration();
        Type repotype = typeof(RepositoryEndpoint<>).MakeGenericType(ContextType);
        int size = PoolSize - Count;
        for (int i = 0; i < size; i++)
        {
            IRepositoryContext repo = repotype.New<IRepositoryContext>(this);
            Add(repo);
        }
    }

    public void CreatePool<TContext>()
    {
        SnapshotConfiguration();
        Type repotype = typeof(RepositoryEndpoint<>).MakeGenericType(typeof(TContext));
        int size = PoolSize - Count;
        for (int i = 0; i < size; i++)
        {
            IRepositoryContext repo = repotype.New<IRepositoryContext>(this);
            Add(repo);
        }
    }

    public new ValueTask DisposeAsync()
    {
        return new ValueTask(Task.Run(() => Dispose(true)));
    }

    public TContext GetContext<TContext>() where TContext : DbContext
    {
        return (TContext)Context;
    }

    public virtual void Lease(IRepositoryContext destContext)
    {
        if (Pooled)
        {
            IRepositoryContext rentContext = Rent();
            DbContext dbcontext = (DbContext)rentContext.InnerContext;
            ChangeTracker changeTracker = dbcontext.ChangeTracker;

            rentContext.ContextLease = destContext;
            destContext.ContextLease = rentContext;
            destContext.InnerContext = dbcontext;

            if (_configurationSnapshot?.AutoDetectChangesEnabled != null)
            {
                changeTracker.AutoDetectChangesEnabled = _configurationSnapshot
                    .AutoDetectChangesEnabled
                    .Value;
                changeTracker.QueryTrackingBehavior = _configurationSnapshot
                    .QueryTrackingBehavior
                    .Value;
                changeTracker.LazyLoadingEnabled = _configurationSnapshot
                    .LazyLoadingEnabled
                    .Value;
                changeTracker.CascadeDeleteTiming = _configurationSnapshot
                    .CascadeDeleteTiming
                    .Value;
                changeTracker.DeleteOrphansTiming = _configurationSnapshot
                    .DeleteOrphansTiming
                    .Value;
            }
            else
            {
                ((IResettableService)changeTracker)?.ResetState();
            }

            DatabaseFacade database = dbcontext?.Database;

            if (database != null)
            {
                database.AutoTransactionsEnabled =
                    _configurationSnapshot?.AutoTransactionsEnabled == null
                    || _configurationSnapshot.AutoTransactionsEnabled.Value;
            }
        }
        else
        {
            if (Count > 0)
            {
                destContext.ContextPool = this;
            }

            destContext.InnerContext = CreateContext();
        }
        disposedValue = false;
    }

    public virtual Task LeaseAsync(IRepositoryContext lease, CancellationToken token = default)
    {
        return Task.Run(() => Lease(lease), token);
    }

    public virtual bool Release()
    {
        if (Leased)
        {
            IRepositoryContext destContext = ContextLease;
            destContext.ContextLease = null;
            destContext.InnerContext = null;
            destContext = null;
            ContextLease = null;

            Return();

            return true;
        }
        return false;
    }

    public void ReleaseAccess()
    {
        _pass.Release();
        _access.Set();
    }

    public virtual Task<bool> ReleaseAsync(CancellationToken token = default)
    {
        return Task.Run(
            () =>
            {
                if (Leased)
                {
                    IRepositoryContext destContext = ContextLease;
                    destContext.ContextLease = null;
                    destContext.InnerContext = null;
                    destContext = null;
                    ContextLease = null;

                    _ = ReturnAsync();

                    return true;
                }
                return false;
            },
            token
        );
    }

    public virtual IRepositoryContext Rent()
    {
        if (TryDequeue(out IRepositoryContext context))
        {
            return context;
        }

        return (IRepositoryContext)
            typeof(IRepositoryEndpoint<>).MakeGenericType(ContextType).New(this);
    }

    public virtual void ResetState()
    {
        ((IDbContextDependencies)Context).StateManager.ResetState();
        ((IResettableService)((DataStoreContext)Context).ChangeTracker).ResetState();
    }

    public virtual Task ResetStateAsync(CancellationToken token = default)
    {
        return Task.Run(
            async () =>
            {
                await ((IDbContextDependencies)Context).StateManager.ResetStateAsync(token);
                await (
                    (IResettableService)((DataStoreContext)Context).ChangeTracker
                ).ResetStateAsync(token);
            },
            token
        );
    }

    public virtual void Return()
    {
        ResetState();
        ContextPool.Add((IRepositoryContext)this);
    }

    public virtual Task ReturnAsync(CancellationToken token = default)
    {
        return Task.Run(
            () =>
            {
                ResetStateAsync();
                ContextPool.Add((IRepositoryContext)this);
            },
            token
        );
    }

    public virtual Task<int> Save(
        bool asTransaction,
        CancellationToken token = default
    )
    {
        return Context.Save(asTransaction, token);
    }

    public object EntitySet<TEntity>() where TEntity : class, IUniqueIdentifiable
    {
        return Context.EntitySet<TEntity>();
    }

    public object EntitySet(Type entityType)
    {
        return Context.EntitySet(entityType);
    }

    public void SnapshotConfiguration()
    {
        ChangeTracker _changeTracker = ((DataStoreContext)Context).ChangeTracker;
        _configurationSnapshot = new DbContextConfigurationSnapshot(
            _changeTracker?.AutoDetectChangesEnabled,
            _changeTracker?.QueryTrackingBehavior,
            ((DataStoreContext)Context).Database?.AutoTransactionsEnabled,
            _changeTracker?.LazyLoadingEnabled,
            _changeTracker?.CascadeDeleteTiming,
            _changeTracker?.DeleteOrphansTiming
        );
    }

    protected override async void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                await Save(true);

                await ReleaseAsync();

                InnerContext = null;
                contextType = null;
                Options = null;
                servicecode.Dispose();
            }

            disposedValue = true;
        }
    }
}
