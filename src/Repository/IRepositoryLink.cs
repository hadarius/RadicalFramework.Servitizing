using Radical.Servitizing.Data.Service.Link;
using Radical.Servitizing.Data.Store;
using Radical.Servitizing.Repository.Client.Linked;

namespace Radical.Servitizing.Repository;

using Entity;

public interface IRepositoryLink<TStore, TOrigin, TTarget> : ILinkedRepository<TStore, TTarget>,
                 IDataServiceLink<TOrigin, TTarget>, ILinkedObject<TStore, TOrigin>
                 where TOrigin : Entity where TTarget : Entity where TStore : IDataServiceStore
{
}
