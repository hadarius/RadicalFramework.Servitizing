using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using Radical.Servitizing.Repository.Endpoint;
using Radical.Servitizing.Repository;

namespace Radical.Servitizing.Data.Store;

using Configuration;

public class DataStoreContextFactory<TContext> : IDesignTimeDbContextFactory<TContext>, IDbContextFactory<TContext> where TContext : DbContext
{
    public TContext CreateDbContext(string[] args)
    {
        var config = new ServiceConfiguration();
        var configEndpoint = config.Endpoint(typeof(TContext).FullName);
        var provider = config.EndpointProvider(configEndpoint);
        RepositoryEndpointOptionsBuilder.AddEntityServicesForDb(provider);
        var options = RepositoryEndpointOptionsBuilder.BuildOptions<TContext>(provider,
                                                                       config.EndpointConnectionString(configEndpoint)).Options;
        return typeof(TContext).New<TContext>(options);
    }

    public TContext CreateDbContext()
    {
        if (RepositoryManager.TryGetEndpoint<TContext>(out var endpoint))
            return endpoint.CreateContext<TContext>();
        return null;
    }
}