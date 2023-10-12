using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Radical.Uniques;
using Radical.Servitizing.Data.Store;

namespace Radical.Servitizing.Repository.Endpoint
{
    public interface IRepositoryEndpoint<TStore, TEntity> : IRepositoryEndpoint where TEntity : class, IUniqueIdentifiable
    {
        IQueryable<TEntity> FromSql(string sql, params object[] parameters);

        DbSet<TEntity> EntitySet();
    }

    public interface IRepositoryEndpoint : IRepositoryContextPool
    {
        IDataStoreContext CreateContext(DbContextOptions options);

        IDataStoreContext CreateContext(Type contextType, DbContextOptions options);

        object EntitySet<TEntity>() where TEntity : class, IUniqueIdentifiable;

        object EntitySet(Type entityType);

        IDataStoreContext Context { get; }

        DbContextOptions Options { get; }
    }

    public interface IRepositoryEndpoint<TContext> : IRepositoryContextPool<TContext>, IDesignTimeDbContextFactory<TContext>, IDbContextFactory<TContext>, IRepositoryEndpoint
        where TContext : DbContext
    {
        TContext CreateContext(DbContextOptions<TContext> options);

        new DbContextOptions<TContext> Options { get; }
    }
}
