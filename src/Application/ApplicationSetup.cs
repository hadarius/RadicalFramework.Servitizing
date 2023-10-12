using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Server;
using Radical.Logging;
using Radical.Series;
using System;
using System.Linq;
using Radical.Servitizing;
using Radical.Servitizing.Repository;

namespace Radical.Servitizing.Application
{
    public partial class ApplicationSetup : IApplicationSetup
    {
        public static bool externalProviderUsed;

        IHostBuilder app;
        IHostEnvironment env;

        public ApplicationSetup(IHostBuilder application) { app = application; }

        public ApplicationSetup(IHostBuilder application, IHostEnvironment environment, bool useSwagger)
        {
            app = application;
            env = environment; 
        }

        public ApplicationSetup(IHostBuilder application, IHostEnvironment environment, string[] apiVersions = null)
        {
            app = application;
            env = environment;
        }

        public virtual IApplicationSetup RebuildProviders()
        {
            UseInternalProvider();
            return this;
        }

        public IApplicationSetup UseDataServices()
        {
            this.LoadOpenDataEdms().ConfigureAwait(true);
            return this;
        }

        public IApplicationSetup UseDataMigrations()
        {
            using (IServiceScope scope = ServiceManager.GetProvider().CreateScope())
            {
                try
                {
                    IServicer us = scope.ServiceProvider.GetRequiredService<IServicer>();
                    us.GetEndpoints().ForEach(e => ((DbContext)e.Context).Database.Migrate());
                }
                catch (Exception ex)
                {
                    this.Error<Applog>("Data migration initial create - unable to connect the database engine", null, ex);
                }
            }

            return this;
        }

        public virtual IApplicationSetup UseInternalProvider()
        {
            IServiceManager sm = ServiceManager.GetManager();
            sm.Registry.MergeServices();
            app.UseServiceProviderFactory(ServiceManager.GetServiceFactory());
            externalProviderUsed = false;
            return this;
        }      
    }
}