using Radical.Series;
using Radical.Servitizing.Data.Cache;
using Radical.Servitizing.Data.Mapper;
using Radical.Uniques;
using System;

namespace Radical.Servitizing.Data.Store;

using Invoking;

public class StoreCache<TStore> : DataCache, IStoreCache<TStore>
{
    public StoreCache(IDataCache cache)
    {
        if (base.cache == null || this.cache == null)
        {
            Mapper = ServiceManager.GetObject<IDataMapper>();
            base.cache = cache;
            uint seed = typeof(TStore).UniqueKey32();
            TypeKey = seed;
            if (!base.Catalog.TryGet(seed, out IUnique deck))
            {
                deck = new TypedRegistry<IUnique>();
                base.Catalog.Add(seed, deck);
            }
            this.cache = (ITypedSeries<IUnique>)deck;
        }
    }

    public StoreCache(TimeSpan? lifeTime = null, Invoker callback = null, int capacity = 257) : base(
        lifeTime,
        callback,
        capacity)
    {
        if (cache == null)
        {
            uint seed = typeof(TStore).UniqueKey32();
            TypeKey = seed;
            if (!base.Catalog.TryGet(seed, out IUnique deck))
            {
                deck = new TypedRegistry<IUnique>();
                base.Catalog.Add(seed, deck);
            }
            cache = (ITypedSeries<IUnique>)deck;
        }
    }

    protected override ITypedSeries<IUnique> cache { get; set; }

    public override ITypedSeries<IUnique> Catalog => cache;

    public IDataMapper Mapper { get; set; }
}
