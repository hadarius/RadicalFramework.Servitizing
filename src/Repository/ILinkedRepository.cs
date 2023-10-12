using Microsoft.OData.Client;
using Radical.Servitizing.Data.Service;
using Radical.Uniques;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Radical.Servitizing.Repository;

public interface ILinkedRepository<TStore, TEntity> : ILinkedRepository<TEntity> where TEntity : class, IUniqueIdentifiable
{
}

public interface ILinkedRepository<TEntity> : IRepository<TEntity> where TEntity : class, IUniqueIdentifiable
{
    OpenDataService Context { get; }

    new DataServiceQuery<TEntity> Query { get; }

    DataServiceQuerySingle<TEntity> QuerySingle(params object[] keys);

    Task<IEnumerable<TEntity>> FindMany(params object[] keys);

    new Task<TEntity> Find(params object[] keys);
}