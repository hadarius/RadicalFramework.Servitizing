using System;
using Radical.Servitizing.Data.Store;

namespace Radical.Servitizing.Entity;

public interface IEntityCache<TStore, TEntity> : IStoreCache<TStore>
{
}