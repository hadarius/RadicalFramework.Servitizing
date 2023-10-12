using System;
using Radical.Series;
using Radical.Servitizing.Data.Cache;
using Radical.Servitizing.Data.Mapper;

namespace Radical.Servitizing.Data.Store;

using Uniques;

public interface IStoreCache<TStore> : IDataCache
{
    IDataMapper Mapper { get; set; }
    ITypedSeries<IUnique> Catalog { get; }
}