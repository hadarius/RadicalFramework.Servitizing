using Microsoft.EntityFrameworkCore.Storage;
using Radical.Series;
using Radical.Servitizing.Entity;
using Radical.Servitizing.Repository.Pagination;
using Radical.Uniques;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Radical.Servitizing.Repository
{
    public interface IStoreRepository<TStore, TEntity> : IStoreRepository<TEntity> where TEntity : class, IUniqueIdentifiable
    {
    }

    public interface IStoreRepository<TEntity> : IPage<TEntity>, IRepository<TEntity> where TEntity : class, IUniqueIdentifiable
    {
        IDbContextTransaction BeginTransaction();
        Task<IDbContextTransaction> BeginTransactionAsync();
        void CommitTransaction(IDbContextTransaction transaction);
        Task CommitTransaction(Task<IDbContextTransaction> transaction);

        IAsyncEnumerable<TEntity> AddByAsync<TModel>(IEnumerable<TModel> model);
        IAsyncEnumerable<TEntity> AddByAsync<TModel>(IEnumerable<TModel> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);

        Task<IEnumerable<TEntity>> AddBy<TModel>(IEnumerable<TModel> model);
        Task<IEnumerable<TEntity>> AddBy<TModel>(IEnumerable<TModel> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        Task<TEntity> AddBy<TModel>(TModel model);
        Task<TEntity> AddBy<TModel>(TModel model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);

        new IPage<TEntity> AsPage(int pageIndex, int pageSize, int indexFrom = 0);

        IAsyncEnumerable<TEntity> DeleteByAsync<TModel>(IEnumerable<TModel> model);
        IAsyncEnumerable<TEntity> DeleteByAsync<TModel>(IEnumerable<TModel> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);

        IEnumerable<TEntity> DeleteBy<TModel>(IEnumerable<TModel> model);
        IEnumerable<TEntity> DeleteBy<TModel>(IEnumerable<TModel> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);
        Task<TEntity> DeleteBy<TModel>(TModel model);
        Task<TEntity> DeleteBy<TModel>(TModel model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate);

        new Task<IPagedSet<TEntity>> Filter(Expression<Func<TEntity, bool>> predicate);
        new Task<IPagedSet<TEntity>> Filter(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders);
        new Task<IPagedSet<TEntity>> Filter(Expression<Func<TEntity, bool>> predicate, EntitySortExpression<TEntity> sortTerms);
        new Task<IPagedSet<TEntity>> Filter(Expression<Func<TEntity, bool>> predicate, EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        new Task<IPagedSet<TEntity>> Filter(EntitySortExpression<TEntity> sortTerms);
        new Task<IPagedSet<TEntity>> Filter(EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);

        Task<IPagedSet<TModel>> Filter<TModel, TResult>(Expression<Func<TEntity, TResult>> selector) where TResult : class;
        Task<IPagedSet<TModel>> Filter<TModel, TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate) where TResult : class;
        Task<IPagedSet<TModel>> Filter<TModel, TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<IPagedSet<TModel>> Filter<TModel, TResult>(Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;

        Task<IList<TModel>> Filter<TModel, TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector) where TResult : class;
        Task<IList<TModel>> Filter<TModel, TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate) where TResult : class;
        Task<IList<TModel>> Filter<TModel, TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<IList<TModel>> Filter<TModel, TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<IList<TModel>> Filter<TModel, TResult>(int skip, int take, Expression<Func<TEntity, TResult>> selector, EntitySortExpression<TEntity> sortTerms, Expression<Func<TEntity, bool>> predicate) where TResult : class;

        new Task<IPagedSet<TModel>> Filter<TModel>(Expression<Func<TEntity, bool>> predicate);
        new Task<IPagedSet<TModel>> Filter<TModel>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders);
        new Task<IPagedSet<TModel>> Filter<TModel>(Expression<Func<TEntity, bool>> predicate, EntitySortExpression<TEntity> sortTerms);
        new Task<IPagedSet<TModel>> Filter<TModel>(Expression<Func<TEntity, bool>> predicate, EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);

        Task<ISeries<TModel>> Filter<TModel>(int skip, int take, Expression<Func<TEntity, bool>> predicate);
        Task<ISeries<TModel>> Filter<TModel>(int skip, int take, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders);
        Task<ISeries<TModel>> Filter<TModel>(int skip, int take, Expression<Func<TEntity, bool>> predicate, EntitySortExpression<TEntity> sortTerms);
        Task<ISeries<TModel>> Filter<TModel>(int skip, int take, Expression<Func<TEntity, bool>> predicate, EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        Task<ISeries<TModel>> Filter<TModel>(int skip, int take, EntitySortExpression<TEntity> sortTerms);
        Task<ISeries<TModel>> Filter<TModel>(int skip, int take, EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);

        IAsyncEnumerable<TModel> FilterAsync<TModel>(int skip, int take, EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        IAsyncEnumerable<TModel> FilterAsync<TModel>(int skip, int take, Expression<Func<TEntity, bool>> predicate, EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);

        Task<IList<TModel>> Filter<TModel>(IQueryable<TEntity> query);
        Task<IList<TEntity>> Filter<TModel>(IQueryable<TModel> query);

        new Task<IPagedSet<TModel>> Filter<TModel>(EntitySortExpression<TEntity> sortTerms);
        new Task<IPagedSet<TModel>> Filter<TModel>(EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);

        Task<IPagedSet<TResult>> Filter<TResult>(Expression<Func<TEntity, TResult>> selector) where TResult : class;
        Task<IPagedSet<TResult>> Filter<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate) where TResult : class;
        Task<IPagedSet<TResult>> Filter<TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<IPagedSet<TResult>> Filter<TResult>(Expression<Func<TEntity, TResult>> selector, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;

        Task<TModel> Find<TModel, TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate) where TResult : class;
        Task<TModel> Find<TModel, TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<TModel> Find<TModel, TResult>(Expression<Func<TEntity, TResult>> selector, object[] keys, params Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<TModel> Find<TModel>(Expression<Func<TEntity, bool>> predicate, bool reverse);
        Task<TModel> Find<TModel>(Expression<Func<TEntity, bool>> predicate, bool reverse, params Expression<Func<TEntity, object>>[] expanders);
        Task<TModel> Find<TModel>(object[] keys, params Expression<Func<TEntity, object>>[] expanders);
        Task<TModel> Find<TModel>(params object[] keys);
        IQueryable<TModel> FindOneAsync<TModel>(object[] keys, params Expression<Func<TEntity, object>>[] expanders) where TModel : class, IUnique;
        IQueryable<TModel> FindOneAsync<TModel>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] expanders) where TModel : class, IUnique;

        new Task<IPagedSet<TEntity>> Get(params Expression<Func<TEntity, object>>[] expanders);
        Task<IList<TModel>> Get<TModel, TResult>(Expression<Func<TEntity, TResult>> selector) where TResult : class;
        Task<IList<TModel>> Get<TModel, TResult>(Expression<Func<TEntity, TResult>> selector, Expression<Func<TEntity, object>>[] expanders) where TResult : class;
        Task<ISeries<TModel>> Get<TModel>(int skip, int take, params Expression<Func<TEntity, object>>[] expanders);
        Task<ISeries<TModel>> Get<TModel>(int skip, int take, EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders);
        new Task<IPagedSet<TModel>> Get<TModel>(params Expression<Func<TEntity, object>>[] expanders);
        IQueryable<TModel> GetQuery<TModel>(params Expression<Func<TEntity, object>>[] expanders) where TModel : class;
        IQueryable<TModel> GetQuery<TModel>(EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) where TModel : class;
        Task<IQueryable<TModel>> GetQueryAsync<TModel>(params Expression<Func<TEntity, object>>[] expanders) where TModel : class;
        Task<IQueryable<TModel>> GetQueryAsync<TModel>(EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) where TModel : class;
        IAsyncEnumerable<TModel> GetAsync<TModel>(int skip, int take, params Expression<Func<TEntity, object>>[] expanders) where TModel : class;
        IAsyncEnumerable<TModel> GetAsync<TModel>(int skip, int take, EntitySortExpression<TEntity> sortTerms, params Expression<Func<TEntity, object>>[] expanders) where TModel : class;

        Task<ISeries<TModel>> HashMap<TModel>(IEnumerable<TEntity> entity, IEnumerable<TModel> model);
        Task<ISeries<TEntity>> HashMap<TModel>(IEnumerable<TModel> model, IEnumerable<TEntity> entity);
        Task<IList<TModel>> Map<TModel>(IEnumerable<TEntity> entity, IEnumerable<TModel> model);
        Task<IList<TEntity>> Map<TModel>(IEnumerable<TModel> model, IEnumerable<TEntity> entity);
        Task<TModel> Map<TModel>(TEntity entity, TModel model);
        Task<TEntity> Map<TModel>(TModel model, TEntity entity);
        Task<IList<TEntity>> MapFrom<TModel>(IEnumerable<TModel> model);
        Task<ISeries<TEntity>> HashMapFrom<TModel>(IEnumerable<TModel> model);
        Task<TModel> MapFrom<TModel>(object model);
        Task<TEntity> MapFrom<TModel>(TModel model);
        Task<IList<TModel>> MapTo<TModel>(IEnumerable<object> entity);
        Task<IList<TModel>> MapTo<TModel>(IEnumerable<TEntity> entity);
        Task<ISeries<TModel>> HashMapTo<TModel>(IEnumerable<object> entity);
        Task<ISeries<TModel>> HashMapTo<TModel>(IEnumerable<TEntity> entity);
        Task<TModel> MapTo<TModel>(object entity);
        Task<TModel> MapTo<TModel>(TEntity entity);

        IAsyncEnumerable<TEntity> PatchByAsync<TModel>(IEnumerable<TModel> entity) where TModel : class, IUniqueIdentifiable;
        IAsyncEnumerable<TEntity> PatchByAsync<TModel>(IEnumerable<TModel> models, Func<TModel, Expression<Func<TEntity, bool>>> predicate) where TModel : class, IUniqueIdentifiable;

        IEnumerable<TEntity> PatchBy<TModel>(IEnumerable<TModel> entity) where TModel : class, IUniqueIdentifiable;
        IEnumerable<TEntity> PatchBy<TModel>(IEnumerable<TModel> models, Func<TModel, Expression<Func<TEntity, bool>>> predicate) where TModel : class, IUniqueIdentifiable;
        Task<TEntity> PatchBy<TModel>(TModel model) where TModel : class, IUniqueIdentifiable;
        Task<TEntity> PatchBy<TModel>(TModel model, Func<TModel, Expression<Func<TEntity, bool>>> predicate) where TModel : class, IUniqueIdentifiable;
        Task<TEntity> PatchBy<TModel>(TModel model, params object[] keys) where TModel : class, IUniqueIdentifiable;

        IAsyncEnumerable<TEntity> SetByAsync<TModel>(IEnumerable<TModel> entity) where TModel : class, IUniqueIdentifiable;
        IAsyncEnumerable<TEntity> SetByAsync<TModel>(IEnumerable<TModel> model, Func<TModel, Expression<Func<TEntity, bool>>> predicate, params Func<TModel, Expression<Func<TEntity, bool>>>[] conditions) where TModel : class, IUniqueIdentifiable;

        IEnumerable<TEntity> SetBy<TModel>(IEnumerable<TModel> entity) where TModel : class, IUniqueIdentifiable;
        IEnumerable<TEntity> SetBy<TModel>(IEnumerable<TModel> model, Func<TModel, Expression<Func<TEntity, bool>>> predicate, params Func<TModel, Expression<Func<TEntity, bool>>>[] conditions) where TModel : class, IUniqueIdentifiable;
        Task<TEntity> SetBy<TModel>(TModel model) where TModel : class, IUniqueIdentifiable;
        Task<TEntity> SetBy<TModel>(TModel model, Func<TModel, Expression<Func<TEntity, bool>>> predicate, params Func<TModel, Expression<Func<TEntity, bool>>>[] conditions) where TModel : class, IUniqueIdentifiable;
        Task<TEntity> SetBy<TModel>(TModel model, params object[] keys) where TModel : class, IUniqueIdentifiable;

        IEnumerable<TEntity> PutBy<TModel>(IEnumerable<TModel> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions);
        Task<TEntity> PutBy<TModel>(TModel model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions);

        IAsyncEnumerable<TEntity> PutByAsync<TModel>(IEnumerable<TModel> model, Func<TEntity, Expression<Func<TEntity, bool>>> predicate, params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions);
    }
}