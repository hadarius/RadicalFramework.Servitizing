using Radical.Servitizing.Data.Mapper;
using Radical.Servitizing.Data.Store;
using Radical.Servitizing.Repository.Client;
using Radical.Servitizing.Repository.Endpoint;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Radical.Servitizing.Repository;

using Entity;

public interface IRepositoryManager
{
    IDataMapper Mapper { get; }

    Task AddClientPools();
    Task AddEndpointPools();

    ILinkedRepository<TEntity> load<TEntity>() where TEntity : Entity;
    ILinkedRepository<TEntity> Load<TEntity>() where TEntity : Entity;
    ILinkedRepository<TEntity> Load<TEntity>(Type contextType) where TEntity : Entity;
    ILinkedRepository<TEntity> load<TStore, TEntity>() where TStore : IDatabaseStore where TEntity : Entity;
    ILinkedRepository<TEntity> Load<TStore, TEntity>() where TStore : IDatabaseStore where TEntity : Entity;

    IRepositoryClient GetClient<TStore, TEntity>() where TEntity : Entity;
    IEnumerable<IRepositoryClient> GetClients();
    IRepositoryEndpoint GetEndpoint<TStore, TEntity>() where TEntity : Entity;
    IEnumerable<IRepositoryEndpoint> GetEndpoints();
    IStoreRepository<TEntity> use<TEntity>() where TEntity : Entity;
    IStoreRepository<TEntity> Use<TEntity>() where TEntity : Entity;
    IStoreRepository<TEntity> Use<TEntity>(Type contextType) where TEntity : Entity;
    IStoreRepository<TEntity> use<TStore, TEntity>()
        where TStore : IDatabaseStore
        where TEntity : Entity;
    IStoreRepository<TEntity> Use<TStore, TEntity>()
        where TStore : IDatabaseStore
        where TEntity : Entity;
}