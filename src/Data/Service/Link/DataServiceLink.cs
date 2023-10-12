using Radical.Servitizing.Data.Store;
using Radical.Uniques;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Radical.Servitizing.Data.Service.Link;

using Instant.Rubrics;

public abstract class DataServiceLink<TOrigin, TTarget, TMiddle> : DataServiceLinkBase, IServiceLink<TOrigin, TTarget, TMiddle>
    where TOrigin : class, IUniqueIdentifiable where TTarget : class, IUniqueIdentifiable
{
    public DataServiceLink()
    {
        var key = typeof(TTarget).Name.UniqueBytes64();
        var seed = typeof(TOrigin).FullName.UniqueKey32();
        serialcode = new Uscn(key, seed);
        Name = typeof(TOrigin).Name + '_' + typeof(TTarget).Name;

        OpenDataServiceRegistry.Links.Add(TypeKey, this);

        OpenDataServiceRegistry.Links.Add(typeof(TTarget).Name.UniqueKey64(seed), this);

        ServiceManager.GetManager().Registry.AddObject<IDataServiceLink<TOrigin, TTarget>>(this);
    }

    public virtual string Name { get; set; }

    public virtual Expression<Func<TOrigin, object>> OriginKey { get; set; }
    public virtual Expression<Func<TMiddle, object>> MiddleKey { get; set; }
    public virtual Expression<Func<TTarget, object>> TargetKey { get; set; }

    public virtual Func<TOrigin, Expression<Func<TTarget, bool>>> Predicate { get; set; }

    public virtual Expression<Func<TOrigin, IEnumerable<TMiddle>>> MiddleSet { get; set; }

    public abstract Expression<Func<TTarget, bool>> CreatePredicate(object entity);

    public override MemberRubric LinkedMember => DataStoreRegistry.GetLinkedMember<TOrigin, TTarget>();

}
