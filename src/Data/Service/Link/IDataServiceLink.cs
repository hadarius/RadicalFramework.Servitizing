using Radical.Instant;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Radical.Servitizing.Data.Service.Link;

using Uniques;
using Instant.Rubrics;

public interface IDataServiceLink : IUnique
{
    Towards Towards { get; set; }

    MemberRubric LinkedMember { get; }
}

public interface IDataServiceLink<TOrigin, TTarget> : IDataServiceLink where TOrigin : class, IUniqueIdentifiable where TTarget : class, IUniqueIdentifiable
{
    Expression<Func<TOrigin, object>> OriginKey { get; set; }
    Expression<Func<TTarget, object>> TargetKey { get; set; }

    Func<TOrigin, Expression<Func<TTarget, bool>>> Predicate { get; set; }

    Expression<Func<TTarget, bool>> CreatePredicate(object entity);
}

public interface IServiceLink<TOrigin, TTarget, TMiddle> : IDataServiceLink<TOrigin, TTarget> where TOrigin : class, IUniqueIdentifiable where TTarget : class, IUniqueIdentifiable
{
    Expression<Func<TMiddle, object>> MiddleKey { get; set; }

    Expression<Func<TOrigin, IEnumerable<TMiddle>>> MiddleSet { get; set; }
}
