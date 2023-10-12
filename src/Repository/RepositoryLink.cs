using Microsoft.OData.Client;
using Radical.Instant;
using System.Linq.Expressions;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Radical.Servitizing.Data.Service.Link;
using Radical.Servitizing.Data.Service;
using Radical.Servitizing.Data.Store;
using Radical.Servitizing.Repository.Client.Linked;

namespace Radical.Servitizing.Repository;

using Entity;
using Instant.Rubrics;

public class RepositoryLink<TStore, TOrigin, TTarget> : LinkedRepository<TStore, TTarget>, IRepositoryLink<TStore, TOrigin, TTarget>
    where TOrigin : Entity
    where TTarget : Entity
    where TStore : IDataServiceStore
{
    IDataServiceLink<TOrigin, TTarget> relation;

    public RepositoryLink(
        IRepositoryContextPool<OpenDataService<TStore>> pool,
        IEntityCache<TStore, TTarget> cache,
        IDataServiceLink<TOrigin, TTarget> relation,
        ILinkedSynchronizer synchronizer) : base(pool, cache)
    {
        this.relation = relation;
        Synchronizer = synchronizer;
    }

    public Expression<Func<TTarget, bool>> CreatePredicate(object entity)
    { return relation.CreatePredicate(entity); }

    public void Load(object origin) { Load(origin, dsContext); }

    public void Load<T>(IEnumerable<T> origins, OpenDataService context) where T : class
    { origins.DoEach((o) => Load(o, context)); }

    public void Load(object origin, OpenDataService context)
    {
        IEntity _entity = (IEntity)origin;
        int rubricId = LinkedMember.RubricId;

        Expression<Func<TTarget, bool>> predicate = CreatePredicate(origin);
        if (predicate != null)
        {
            ILinkedSet<TTarget> dso;
            switch (Towards)
            {
                case Towards.ToSingle:
                    DataServiceQuery<TTarget> query = context.CreateQuery<TTarget>(typeof(TTarget).Name);
                    Synchronizer.AcquireLinker();
                    _entity[rubricId] = query.FirstOrDefault(predicate);
                    Synchronizer.ReleaseLinker();
                    break;
                case Towards.ToSet:
                    dso = new LinkedSet<TTarget>(context);
                    dso.LoadCompleted += Synchronizer.OnLinked;
                    _entity[rubricId] = dso;
                    dso.LoadAsync(predicate);
                    Synchronizer.AcquireLinker();
                    break;
                case Towards.SetToSet:
                    dso = new LinkedSet<TTarget>(context);
                    dso.LoadCompleted += Synchronizer.OnLinked;
                    _entity[rubricId] = dso;
                    dso.LoadAsync(predicate);
                    Synchronizer.AcquireLinker();
                    break;
                default:
                    break;
            }
        }
    }

    public async Task LoadAsync(object origin) { await Task.Run(() => Load(origin, dsContext), Cancellation); }

    public async ValueTask LoadAsync(object origin, OpenDataService context, CancellationToken token)
    { await Task.Run(() => Load(origin, context), token); }

    public override Task<int> Save(bool asTransaction, CancellationToken token = default)
    { return ContextLease.Save(asTransaction, token); }

    public IRepository Host { get; set; }

    public bool IsLinked { get; set; }

    public override int LinkedCount { get; set; }

    public MemberRubric LinkedMember => relation.LinkedMember;

    public Expression<Func<TOrigin, object>> OriginKey
    {
        get => relation.OriginKey;
        set => relation.OriginKey = value;
    }

    public Func<TOrigin, Expression<Func<TTarget, bool>>> Predicate
    {
        get => relation.Predicate;
        set => relation.Predicate = value;
    }

    public ILinkedSynchronizer Synchronizer { get; }

    public Expression<Func<TTarget, object>> TargetKey
    {
        get => relation.TargetKey;
        set => relation.TargetKey = value;
    }

    public override Towards Towards => relation.Towards;
}
