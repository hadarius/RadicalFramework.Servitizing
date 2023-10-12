using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Radical.Servitizing.Data.Service.Link;

using Uniques;

public class LinkedOneToSet<TOrigin, TTarget> : DataServiceLink<TOrigin, TTarget, ILinkedSet<TTarget>> where TOrigin : class, IUniqueIdentifiable where TTarget : class, IUniqueIdentifiable
{
    private Func<TTarget, object> targetKey;
    private Func<TOrigin, object> originKey;

    public LinkedOneToSet() : base()
    {
    }
    public LinkedOneToSet(Expression<Func<TOrigin, object>> originkey,
                            Expression<Func<TTarget, object>> targetkey)
                                : base()
    {
        Towards = Towards.ToSet;
        OriginKey = originkey;
        TargetKey = targetkey;

        originKey = originkey.Compile();
        targetKey = targetkey.Compile();
    }

    public override Expression<Func<TTarget, bool>> CreatePredicate(object entity)
    {
        return LinqExtension.GetEqualityExpression(TargetKey, originKey, (TOrigin)entity);
    }
}
