using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Radical.Series;
using Radical.Servitizing.Data.Mapper;
using Radical.Servitizing.Data.Service;
using Radical.Servitizing.Data.Store;
using Radical.Servitizing.Repository.Client;
using Radical.Servitizing.Repository.Endpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Radical.Servitizing.Repository;

using Entity;
using Endpoint;
using Client;

public class RepositoryManager : Catalog<IDataStoreContext>, IDisposable, IAsyncDisposable, IRepositoryManager
{
    private new bool disposedValue;
    protected IDataMapper mapper;

    protected static IRepositoryEndpoints Endpoints { get; set; }
    public static IRepositoryClients Clients { get; set; }

    protected IServiceManager Services { get; init; }

    public IDataMapper Mapper
    {
        get => mapper ??= GetMapper();
    }

    static RepositoryManager()
    {
        Endpoints = new RepositoryEndpoints();
        Clients = new RepositoryClients();
    }
    public RepositoryManager() : base()
    {
    }

    public IStoreRepository<TEntity> use<TStore, TEntity>() where TEntity : Entity where TStore : IDatabaseStore
    {
        return Use<TStore, TEntity>();
    }
    public IStoreRepository<TEntity> use<TEntity>() where TEntity : Entity
    {
        return Use<TEntity>();
    }

    public IStoreRepository<TEntity> Use<TEntity>()
    where TEntity : Entity
    {
        return Use<TEntity>(DataStoreRegistry.GetContexts<TEntity>().FirstOrDefault());
    }
    public IStoreRepository<TEntity> Use<TEntity>(Type contextType)
        where TEntity : Entity
    {
        return (IStoreRepository<TEntity>)Services.GetService(typeof(IStoreRepository<,>)
                                                 .MakeGenericType(DataStoreRegistry
                                                 .Stores[contextType],
                                                  typeof(TEntity)));
    }
    public IStoreRepository<TEntity> Use<TStore, TEntity>()
       where TEntity : Entity where TStore : IDatabaseStore
    {
        return Services.GetService<IStoreRepository<TStore, TEntity>>();
    }

    public ILinkedRepository<TEntity> load<TStore, TEntity>() where TEntity : Entity where TStore : IDatabaseStore
    {
        return Load<TStore, TEntity>();
    }
    public ILinkedRepository<TEntity> load<TEntity>() where TEntity : Entity
    {
        return Load<TEntity>();
    }

    public ILinkedRepository<TEntity> Load<TEntity>() where TEntity : Entity
    {
        return Load<TEntity>(OpenDataServiceRegistry.GetContextTypes<TEntity>().FirstOrDefault());
    }
    public ILinkedRepository<TEntity> Load<TEntity>(Type contextType)
       where TEntity : Entity
    {
        return (ILinkedRepository<TEntity>)Services.GetService(typeof(ILinkedRepository<,>)
                                                 .MakeGenericType(OpenDataServiceRegistry
                                                 .Stores[contextType],
                                                  typeof(TEntity)));
    }
    public ILinkedRepository<TEntity> Load<TStore, TEntity>() where TEntity : Entity where TStore : IDatabaseStore
    {
        var result = Services.GetService<ILinkedRepository<TStore, TEntity>>();
        return result;
    }

    public IRepositoryEndpoint GetEndpoint<TStore, TEntity>()
    where TEntity : Entity
    {
        var contextType = DataStoreRegistry.GetContext<TStore, TEntity>();
        return Endpoints.Get(contextType);
    }

    public IRepositoryClient GetClient<TStore, TEntity>()
    where TEntity : Entity
    {
        var contextType = OpenDataServiceRegistry.GetContextType<TStore, TEntity>();

        return Clients.Get(contextType);
    }

    public static void AddClientPool(Type contextType, int poolSize, int minSize = 1)
    {
        if (TryGetClient(contextType, out IRepositoryClient client))
        {
            client.PoolSize = poolSize;
            client.CreatePool();
        }
    }

    public Task AddClientPools()
    {
        return Task.Run(() =>
        {
            foreach (var client in GetClients())
            {
                client.CreatePool();
            }
        });
    }

    public static IRepositoryClient CreateClient(IRepositoryClient client)
    {
        Type repotype = typeof(RepositoryClient<>).MakeGenericType(client.ContextType);
        return (IRepositoryClient)repotype.New(client);
    }
    public static IRepositoryClient<TContext> CreateClient<TContext>(IRepositoryClient<TContext> client)
        where TContext : OpenDataService
    {
        return new RepositoryClient<TContext>(client);
    }
    public static IRepositoryClient<TContext> CreateClient<TContext>(Uri serviceRoot) where TContext : OpenDataService
    {
        return new RepositoryClient<TContext>(serviceRoot);
    }
    public static IRepositoryClient CreateClient(Type contextType, Uri serviceRoot)
    {
        return new RepositoryClient(contextType, serviceRoot);
    }

    public static IRepositoryClient AddClient(IRepositoryClient client)
    {
        if (Clients == null)
            Clients = ServiceManager.GetObject<IRepositoryClients>();
        Clients.Add(client);
        return client;
    }

    public static bool TryGetClient<TContext>(out IRepositoryClient<TContext> endpoint) where TContext : OpenDataService
    {
        return Clients.TryGet(out endpoint);
    }
    public static bool TryGetClient(Type contextType, out IRepositoryClient endpoint)
    {
        return Clients.TryGet(contextType, out endpoint);
    }

    public Task AddEndpointPools()
    {
        return Task.Run(() =>
        {
            foreach (var endpoint in Endpoints)
            {
                endpoint.CreatePool();
            }
        });
    }

    public static void AddEndpointPool(Type contextType, int poolSize, int minSize = 1)
    {
        if (TryGetEndpoint(contextType, out IRepositoryEndpoint endpoint))
        {
            endpoint.PoolSize = poolSize;
            endpoint.CreatePool();
        }
    }

    public static IRepositoryEndpoint<TContext> CreateEndpoint<TContext>(DbContextOptions<TContext> options) where TContext : DataStoreContext
    {
        return new RepositoryEndpoint<TContext>(options);
    }
    public static IRepositoryEndpoint CreateEndpoint(IRepositoryEndpoint endpoint)
    {
        Type repotype = typeof(RepositoryEndpoint<>).MakeGenericType(endpoint.ContextType);
        return (IRepositoryEndpoint)repotype.New(endpoint);
    }
    public static IRepositoryEndpoint<TContext> CreateEndpoint<TContext>(IRepositoryEndpoint<TContext> endpoint)
        where TContext : DataStoreContext
    {
        return typeof(RepositoryEndpoint<TContext>).New<IRepositoryEndpoint<TContext>>(endpoint);
    }
    public static IRepositoryEndpoint CreateEndpoint(DbContextOptions options)
    {
        return new RepositoryEndpoint(options);
    }

    public static IRepositoryEndpoint AddEndpoint(IRepositoryEndpoint endpoint)
    {
        if (Endpoints == null)
            Endpoints = ServiceManager.GetObject<IRepositoryEndpoints>();
        Endpoints.Add(endpoint);
        return endpoint;
    }

    public static bool TryGetEndpoint<TContext>(out IRepositoryEndpoint<TContext> endpoint) where TContext : DbContext
    {
        return Endpoints.TryGet(out endpoint);
    }
    public static bool TryGetEndpoint(Type contextType, out IRepositoryEndpoint endpoint)
    {
        return Endpoints.TryGet(contextType, out endpoint);
    }

    public IEnumerable<IRepositoryEndpoint> GetEndpoints()
    {
        return Endpoints;
    }

    public IEnumerable<IRepositoryClient> GetClients()
    {
        return Clients;
    }

    public static IDataMapper CreateMapper(params Profile[] profiles)
    {
        DataMapper.AddProfiles(profiles);
        return ServiceManager.GetObject<IDataMapper>();
    }
    public static IDataMapper CreateMapper<TProfile>() where TProfile : Profile
    {
        DataMapper.AddProfiles(typeof(TProfile).New<TProfile>());
        return ServiceManager.GetObject<IDataMapper>();
    }

    public static IDataMapper GetMapper()
    {
        return ServiceManager.GetObject<IDataMapper>();
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                base.Dispose(true);
            }
            disposedValue = true;
        }
    }

    public override async ValueTask DisposeAsyncCore()
    {
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }
}
