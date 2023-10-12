using Microsoft.Extensions.DependencyInjection;
using Radical.Servitizing.Data.File.Blob.Container;

namespace Radical.Servitizing.Data.File.Blob;

using Configuration;

public class BlobStoringModule
{
    public void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient(
            typeof(IBlobContainer<>),
            typeof(BlobContainer<>)
        );

        context.Services.AddTransient(
            typeof(IBlobContainer),
            serviceProvider => serviceProvider
                .GetRequiredService<IBlobContainer<DefaultContainer>>()
        );
    }
}