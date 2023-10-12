using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Radical.Series;
using System.Threading.Tasks;
using Radical.Uniques;
using Radical.Servitizing.Data.Store;

namespace Radical.Servitizing.Entity
{
    public class EntityCache<TStore, TEntity> : StoreCache<TStore>, IEntityCache<TStore, TEntity>
        where TEntity : IUnique
    {
        public EntityCache(IStoreCache<TStore> datacache) : base()
        {
            if (base.cache == null || cache == null)
            {
                Mapper = datacache.Mapper;
                base.cache = datacache;
                uint seed = typeof(TEntity).GetProxyEntityTypeKey32();
                if (!base.Catalog.TryGet(seed, out IUnique deck))
                {
                    deck = new TypedRegistry<IUnique>();
                    base.Catalog.Add(seed, deck);
                }
                cache = (ITypedSeries<IUnique>)deck;

                TypeKey = seed;
                base.TypeKey = typeof(TStore).GetProxyEntityTypeKey32();
            }
        }

        protected override ITypedSeries<IUnique> cache { get; set; }

        public ITypedSeries<IUnique> CacheSet()
        {
            return CacheSet<TEntity>();
        }

        public TEntity Lookup(object keys)
        {
            return Lookup<TEntity>(keys);
        }

        public ISeries<IUnique> Lookup(Tuple<string, object> valueNamePair)
        {
            return Lookup<TEntity>(
                (m) => (ISeries<IUnique>)m.Get(valueNamePair.Item2, valueNamePair.Item1.UniqueKey32())
            );
        }

        public ISeries<IUnique> Lookup(Func<ITypedSeries<IUnique>, ISeries<IUnique>> selector)
        {
            return Lookup<TEntity>(selector);
        }

        public TEntity Lookup(TEntity item)
        {
            return Lookup<TEntity>(item);
        }

        public TEntity[] Lookup(object key, params Tuple<string, object>[] valueNamePairs)
        {
            return Lookup<TEntity>(key, valueNamePairs);
        }

        public TEntity[] Lookup(
            Func<ISeries<IUnique>, IUnique> key,
            params Func<ITypedSeries<IUnique>, ISeries<IUnique>>[] selectors
        )
        {
            return Lookup<TEntity>(key, selectors);
        }

        public ISeries<IUnique> Lookup(object key, string propertyNames)
        {
            return Lookup<TEntity>(key, propertyNames);
        }

        public TEntity Lookup(TEntity item, params string[] propertyNames)
        {
            return Lookup<TEntity>(item, propertyNames);
        }

        public IEnumerable<TEntity> Memorize(IEnumerable<TEntity> items)
        {
            return Memorize<TEntity>(items);
        }

        public TEntity Memorize(TEntity item)
        {
            return Memorize<TEntity>(item);
        }

        public TEntity Memorize(TEntity item, params string[] names)
        {
            return Memorize<TEntity>(item, names);
        }

        public async Task<TEntity> MemorizeAsync(TEntity item)
        {
            return await MemorizeAsync<TEntity>(item);
        }

        public async Task<TEntity> MemorizeAsync(TEntity item, params string[] names)
        {
            return await MemorizeAsync<TEntity>(item, names);
        }

        public override ITypedSeries<IUnique> Catalog => cache;

        public override ulong TypeKey { get; set; }
    }
}
