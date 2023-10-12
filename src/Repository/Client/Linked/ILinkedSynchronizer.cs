using Microsoft.OData.Client;

namespace Radical.Servitizing.Repository.Client.Linked
{
    public interface ILinkedSynchronizer
    {
        void AddRepository(IRepository repository);

        void OnLinked(object sender, LoadCompletedEventArgs args);

        void AcquireLinker();

        void ReleaseLinker();

        void AcquireResult();

        void ReleaseResult();
    }
}

