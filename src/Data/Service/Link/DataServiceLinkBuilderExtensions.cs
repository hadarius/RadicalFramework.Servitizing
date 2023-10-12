using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Radical.Servitizing.Data.Service.Link;

using Uniques;

public static class DataServiceLinkBuilderExtensions
{
    public static IEdmModel LinkSetToSet<TOrigin, TMiddle, TTarget>(this IEdmModel builder,
                                                             Expression<Func<TOrigin, IEnumerable<TMiddle>>> middleSet,
                                                             Expression<Func<TMiddle, object>> middlekey,
                                                             Expression<Func<TTarget, object>> targetkey)
                                                          where TOrigin : class, IUniqueIdentifiable
                                                          where TTarget : class, IUniqueIdentifiable
                                                           where TMiddle : class, IUniqueIdentifiable
    {
        new LinkedSetToSet<TOrigin, TTarget, TMiddle>(middleSet, middlekey, targetkey);
        return builder;
    }

    public static IEdmModel LinkToSet<TOrigin, TTarget>(this IEdmModel builder,
                                                             Expression<Func<TOrigin, object>> originkey,
                                                             Expression<Func<TTarget, object>> targetkey)
                                                         where TOrigin : class, IUniqueIdentifiable
                                                         where TTarget : class, IUniqueIdentifiable
    {
        new LinkedOneToSet<TOrigin, TTarget>(originkey, targetkey);
        return builder;
    }

    public static IEdmModel LinkToSingle<TOrigin, TTarget>(this IEdmModel builder,
                                                            Expression<Func<TOrigin, object>> originkey,
                                                             Expression<Func<TTarget, object>> targetkey)
                                                        where TOrigin : class, IUniqueIdentifiable
                                                        where TTarget : class, IUniqueIdentifiable
    {
        new LinkedOneToOne<TOrigin, TTarget>(originkey, targetkey);
        return builder;
    }

}

