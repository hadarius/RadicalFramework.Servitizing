using Microsoft.EntityFrameworkCore.ChangeTracking;
using Radical.Servitizing.Data.Service.Link;
using Radical.Servitizing.Data.Store;
using System.Threading.Tasks;

namespace Radical.Servitizing.Repository.Client.Linked;

using Entity;

public interface ILinkedObject<TStore, TOrigin> : ILinkedObject<TOrigin> where TOrigin : Entity where TStore : IDataServiceStore
{
}

public interface ILinkedObject<TOrigin> : ILinkedObject where TOrigin : Entity
{
}

public interface ILinkedObject : IRepository, IDataServiceLink
{
    bool IsLinked { get; set; }

    IRepository Host { get; set; }

    void Load(object origin);

    Task LoadAsync(object origin);

    void LinkTrigger(object sender, EntityEntryEventArgs e);

    ILinkedSynchronizer Synchronizer { get; }
}
