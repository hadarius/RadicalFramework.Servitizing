using Microsoft.Extensions.DependencyInjection;
using Radical.Servitizing;

namespace Radical.Servitizing
{
    public static class ServiceSetupExtensions
    {
        public static IServiceSetup AddServiceSetup(this IServiceCollection services, IMvcBuilder mvcBuilder = null)
        {
            return new ServiceSetup(services);
        }
    }
}
