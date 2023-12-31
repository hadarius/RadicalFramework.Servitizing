using Microsoft.OData.Client;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Radical.Servitizing.Repository.Client;

using Configuration;
using Data.Service;
using Endpoint;
using Logging;
using Security.Identity;
using Series;
using Uniques;
using Entity;

public class RepositoryClient : Catalog<IRepositoryContext>, IRepositoryClient
{
    protected Uri uri;
    protected Uscn servicecode;
    private new bool disposedValue;
    protected Type contextType;

    public RepositoryClient()
    {
        Site = DataSite.Client;
    }
    public RepositoryClient(Type contextType, IServiceConfiguration config) : this()
    {
        var endpoint = config.Client(contextType.FullName);
        var connectionString = config.ClientConnectionString(contextType.FullName);
        var provider = config.ClientProvider(contextType.FullName);
        PoolSize = config.ClientPoolSize(endpoint);
        InnerContext = CreateContext(contextType, new Uri(connectionString));
    }
    public RepositoryClient(Type contextType, Uri serviceRoot) : this()
    {
        InnerContext = CreateContext(contextType, serviceRoot);
    }
    public RepositoryClient(Type contextType, ClientProvider provider, string connectionString) : this()
    {
        InnerContext = CreateContext(contextType, new Uri(connectionString));
    }
    public RepositoryClient(IRepositoryClient pool) : this()
    {
        PoolSize = pool.PoolSize;
        ContextPool = pool;
        InnerContext = pool.CreateContext();
    }

    public object InnerContext { get; set; }

    public DataSite Site { get; set; }

    public int PoolSize { get; set; }

    public Type ContextType => contextType ??= InnerContext.GetType();

    public virtual Uri Route => uri;

    public virtual OpenDataService Context => (OpenDataService)InnerContext;

    public virtual object CreateContext()
    {
        return ContextType.New<OpenDataService>(uri);
    }
    public virtual object CreateContext(Type contextType, Uri serviceRoot)
    {
        uri ??= serviceRoot;
        this.contextType ??= contextType;
        return (OpenDataService)contextType.New(uri);
    }

    public virtual TContext GetContext<TContext>() where TContext : OpenDataService
    {
        return (TContext)InnerContext;
    }

    public virtual TContext CreateContext<TContext>() where TContext : class
    {
        contextType ??= typeof(TContext);
        return typeof(TContext).New<TContext>(uri);
    }
    public virtual TContext CreateContext<TContext>(Uri serviceRoot) where TContext : OpenDataService
    {
        uri = serviceRoot;
        contextType ??= typeof(TContext);
        return typeof(TContext).New<TContext>(uri);
    }

    public void BuildMetadata()
    {
        Context.CreateServiceModel();
        Context.GetEdmEntityTypes();
    }

    public override ulong Key
    {
        get => servicecode.Key == 0 ? servicecode.Key = Unique.NewKey : servicecode.Key;
        set => servicecode.Key = value;
    }

    public override ulong TypeKey
    {
        get => servicecode.TypeKey == 0 ? servicecode.TypeKey = ContextType.GetProxyEntityTypeKey32() : servicecode.TypeKey;
        set => servicecode.TypeKey = value;
    }

    public IRepositoryContext ContextLease { get; set; }

    public IRepositoryContextPool ContextPool { get; set; }

    public bool Pooled => ContextPool != null;

    public bool Leased => ContextLease != null;

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
                uri = null;
                servicecode.Dispose();
            }

            disposedValue = true;
        }
    }

    public new ValueTask DisposeAsync()
    {
        return new ValueTask(Task.Run(() => Dispose(true)));
    }

    public virtual IRepositoryContext Rent()
    {
        if (TryDequeue(out IRepositoryContext context))
        {
            return context;
        }

        return context = (IRepositoryContext)typeof(IRepositoryClient<>)
                                       .MakeGenericType(ContextType)
                                       .New(this);
    }

    public virtual void Return()
    {
        ResetState();
        ContextPool.Add((IRepositoryContext)this);
    }

    public virtual Task ReturnAsync(CancellationToken token = default)
    {
        return Task.Run(() =>
        {
            ResetStateAsync(token);
            ContextPool.Add((IRepositoryContext)this);
        }, token);
    }

    public virtual void CreatePool()
    {
        Type repotype = typeof(RepositoryClient<>)
                        .MakeGenericType(ContextType);
        var size = PoolSize - Count;
        for (int i = 0; i < size; i++)
        {
            var repo = repotype.New<IRepositoryContext>(this);
            Add(repo);
        }
    }
    public virtual void CreatePool<TContext>()
    {
        Type repotype = typeof(RepositoryClient<>)
                        .MakeGenericType(typeof(TContext));
        var size = PoolSize - Count;
        for (int i = 0; i < size; i++)
        {
            var repo = repotype.New<IRepositoryContext>(this);
            repo.Key = Unique.NewKey;
            Add(repo);
        }
    }

    public virtual void ResetState()
    {
        Context.Entities.ForEach((e) => Context.Detach(e.Entity));
    }

    public virtual Task ResetStateAsync(CancellationToken token = default)
    {
        return Task.Run(() => ResetState(), token);
    }

    public virtual Task<int> Save(bool asTransaction, CancellationToken token = default)
    {
        return saveClient(asTransaction);
    }

    public virtual bool Release()
    {
        if (Leased)
        {
            var destContext = ContextLease;
            destContext.ContextLease = null;
            destContext.InnerContext = null;
            destContext = null;
            ContextLease = null;

            Return();

            return true;
        }
        return false;
    }

    public virtual Task<bool> ReleaseAsync(CancellationToken token = default)
    {
        return Task.Run(() =>
        {
            if (Leased)
            {
                var destContext = ContextLease;
                destContext.ContextLease = null;
                destContext.InnerContext = null;
                destContext = null;
                ContextLease = null;

                _ = ReturnAsync(token);

                return true;
            }
            return false;
        }, token);
    }

    public virtual void Lease(IRepositoryContext destContext)
    {
        if (Pooled)
        {
            var rentContext = Rent();

            rentContext.ContextLease = destContext;
            destContext.ContextLease = rentContext;
            destContext.InnerContext = rentContext.InnerContext;
        }
        else
        {
            if (Count > 0)
                destContext.ContextPool = this;
            destContext.InnerContext = CreateContext();
        }
        disposedValue = false;
    }

    public virtual Task LeaseAsync(IRepositoryContext destContext, CancellationToken token = default)
    {
        return Task.Run(() => Lease(destContext), token);
    }

    private async Task<int> saveClient(bool asTransaction, CancellationToken token = default)
    {
        if (Context.Entities.Any(s => s.State != EntityStates.Unchanged))
        {
            if (asTransaction)
                return await saveAsTransaction(Context, token);
            else
                return await saveChanges(Context, token);
        }
        return 0;
    }

    private async Task<int> saveAsTransaction(OpenDataService context, CancellationToken token = default)
    {
        try
        {
            return (await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset, token)).Count();
        }
        catch (Exception e)
        {
            context.Failure<Datalog>(
                $"Fail on update dataservice as singlechangeset, using context:{context.GetType().Name}, " +
                $"TimeStamp: {DateTime.Now.ToString()}");
        }

        return -1;
    }

    private async Task<int> saveChanges(OpenDataService context, CancellationToken token = default)
    {
        try
        {
            return (await context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.ContinueOnError, token)).Count();
        }
        catch (Exception e)
        {
            context.Failure<Datalog>(
                $"Fail on update dataservice as independent operations, using context:{context.GetType().Name}, " +
                $"TimeStamp: {DateTime.Now.ToString()}");
        }

        return -1;
    }

    public void SnapshotConfiguration()
    {
        throw new NotImplementedException();
    }
}
