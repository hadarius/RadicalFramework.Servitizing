using System.Linq;
using System.Threading.Tasks;

namespace Radical.Servitizing.Repository;

using Application;

public static class RepositoryManagerExtensions
{
    public static async Task LoadOpenDataEdms(this ApplicationSetup app)
    {
        await Task.Run(() =>
        {
            RepositoryManager.Clients.ForEach((client) =>
            {
                client.BuildMetadata();
            });

            ApplicationSetup.AddOpenDataServiceImplementations();
            app.RebuildProviders();
        });
    }

}
