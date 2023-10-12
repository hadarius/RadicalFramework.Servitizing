using Microsoft.Extensions.Hosting;
using Radical.Servitizing;

namespace Radical.Servitizing.Application
{
    public static class ApplicationSetupExtensions
    {
        public static IApplicationSetup UseAppSetup(this IHostBuilder app, IHostEnvironment env)
        {
            return new ApplicationSetup(app, env);
        }

        public static IHostBuilder UseInternalProvider(this IHostBuilder app)
        {
            new ApplicationSetup(app).UseInternalProvider();
            return app;
        }

        public static IHostBuilder RebuildProviders(this IHostBuilder app)
        {
            new ApplicationSetup(app).RebuildProviders();
            return app;
        }
    }
}