using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Radical.Instant;
using System.Linq;
using Radical.Series;
using System.Threading.Tasks;
using Radical.Uniques;
using Radical.Servitizing.Data.Store;

namespace Radical.Servitizing.Data.Cache;

using Invoking;
using Instant.Proxies;
using Instant.Rubrics;
using Entity;

public class DataCache : TypedCache<IUnique>, IDataCache
{
    public DataCache() : this(TimeSpan.FromMinutes(15), null, 259)
    {
    }

    public DataCache(TimeSpan? lifeTime = null, IInvoker callback = null, int capacity = 259) : base(
        lifeTime,
        callback,
        capacity)
    {
        if (cache == null)
        {
            cache = this;
        }
    }

    protected virtual ITypedSeries<IUnique> cache { get; set; }

    protected override T InnerMemorize<T>(T item)
    {
        uint group = typeof(T).GetProxyEntityTypeKey32();
        if (!cache.TryGet(group, out IUnique deck))
        {
            ProxyCreator sleeve = ProxyFactory.GetCreator(typeof(T).GetProxyEntityType(), group);
            sleeve.Create();

            IRubrics keyrubrics = AssignKeyRubrics(sleeve, item);

            IProxy isleeve = item.ToProxy();

            deck = new TypedRegistry<IUnique>();

            foreach (MemberRubric keyRubric in keyrubrics)
            {
                Registry<IUnique> subdeck = new Registry<IUnique>();

                subdeck.Add(item);

                ((ITypedSeries<IUnique>)deck).Put(
                    isleeve[keyRubric.RubricId],
                    keyRubric.RubricName.UniqueKey32(),
                    subdeck);
            }

            cache.Add(group, deck);

            cache.Add(item);

            return item;
        }

        if (!cache.ContainsKey(item))
        {
            ITypedSeries<IUnique> _deck = (ITypedSeries<IUnique>)deck;

            IProxy isleeve = item.ToProxy();

            foreach (MemberRubric keyRubric in isleeve.Rubrics.KeyRubrics)
            {
                if (!_deck.TryGet(
                    isleeve[keyRubric.RubricId],
                    keyRubric.RubricName.UniqueKey32(),
                    out IUnique outdeck))
                {
                    outdeck = new Registry<IUnique>();

                    ((ISeries<IUnique>)outdeck).Put(item);

                    _deck.Put(isleeve[keyRubric.RubricId], keyRubric.RubricName.UniqueKey32(), outdeck);
                }
                else
                {
                    ((ISeries<IUnique>)outdeck).Put(item);
                }
            }
            cache.Add(item);
        }

        return item;
    }

    protected override T InnerMemorize<T>(T item, params string[] names)
    {
        Memorize(item);

        IProxy sleeve = item.ToProxy();

        MemberRubric[] keyrubrics = sleeve.Rubrics.Where(p => names.Contains(p.RubricName)).ToArray();

        ITypedSeries<IUnique> _deck = (ITypedSeries<IUnique>)cache.Get(item.TypeKey);

        foreach (MemberRubric keyRubric in keyrubrics)
        {
            if (!_deck.TryGet(sleeve[keyRubric.RubricId], keyRubric.RubricName.UniqueKey32(), out IUnique outdeck))
            {
                outdeck = new Registry<IUnique>();

                ((ISeries<IUnique>)outdeck).Put(item);

                _deck.Put(sleeve[keyRubric.RubricId], keyRubric.RubricName.UniqueKey32(), outdeck);
            }
            else
            {
                ((ISeries<IUnique>)outdeck).Put(item);
            }
        }

        return item;
    }

    public static IRubrics AssignKeyRubrics(ProxyCreator sleeve, IUnique item)
    {
        if (!sleeve.Rubrics.KeyRubrics.Any())
        {
            IEnumerable<bool[]>[] rk = item.GetIdentities()
                .AsItems()
                .Select(
                    p => p.Key != (int)DbIdentityType.NONE
                        ? p.Value
                            .Select(
                                e => new[]
                                        {
                                            sleeve.Rubrics[e.Name].IsKey = true,
                                            sleeve.Rubrics[e.Name].IsIdentity = true
                                        })
                        : p.Value.Select(h => new[] { sleeve.Rubrics[h.Name].IsIdentity = true }))
                .ToArray();

            sleeve.Rubrics.KeyRubrics.Put(sleeve.Rubrics.Where(p => p.IsIdentity == true).ToArray());
            sleeve.Rubrics.Update();
        }

        return sleeve.Rubrics.KeyRubrics;
    }

    public override T Memorize<T>(T item)
    {
        if (!item.IsEventType())
            return InnerMemorize(item);
        return default;
    }

    public virtual ITypedSeries<IUnique> Catalog => cache;

    public override uint GetValidTypeKey(Type obj)
    {
        return obj.GetProxyEntityTypeKey32();
    }
    public override Type GetValidType(Type obj)
    {
        return obj.GetProxyEntityType();
    }
    public override uint GetValidTypeKey(object obj)
    {
        return obj.GetProxyEntityTypeKey32();
    }
    public override Type GetValidType(object obj)
    {
        return obj.GetProxyEntityType();
    }
}
