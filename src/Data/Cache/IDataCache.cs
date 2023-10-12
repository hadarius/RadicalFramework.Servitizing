using Radical.Series;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Radical.Servitizing.Data.Cache;

using Uniques;

public interface IDataCache : ITypedSeries<IUnique>
{
    ITypedSeries<IUnique> CacheSet<T>() where T : IUnique;

    T Lookup<T>(T item) where T : IUnique;

    ISeries<IUnique> Lookup<T>(Func<ITypedSeries<IUnique>, ISeries<IUnique>> selector) where T : IUnique;

    T Lookup<T>(object keys) where T : IUnique;

    ISeries<IUnique> Lookup<T>(Tuple<string, object> valueNamePair) where T : IUnique;

    T Lookup<T>(T item, params string[] propertyNames) where T : IUnique;

    T[] Lookup<T>(Func<ISeries<IUnique>, IUnique> key, params Func<ITypedSeries<IUnique>, ISeries<IUnique>>[] selectors)
        where T : IUnique;

    T[] Lookup<T>(object key, params Tuple<string, object>[] valueNamePairs) where T : IUnique;

    ISeries<IUnique> Lookup<T>(object key, string propertyNames) where T : IUnique;

    IEnumerable<T> Memorize<T>(IEnumerable<T> items) where T : IUnique;

    T Memorize<T>(T item) where T : IUnique;

    T Memorize<T>(T item, params string[] names) where T : IUnique;

    Task<T> MemorizeAsync<T>(T item) where T : IUnique;

    Task<T> MemorizeAsync<T>(T item, params string[] names) where T : IUnique;

    ITypedSeries<IUnique> Catalog { get; }
}