using Microsoft.Extensions.Options;
using Radical.Servitizing.Data.File.Blob.Container;

namespace Radical.Servitizing.Data.File.Blob
{
    public class DefaultBlobContainerConfigurationProvider : IBlobContainerConfigurationProvider
    {
        protected BlobStoringOptions Options { get; }

        public DefaultBlobContainerConfigurationProvider(IOptions<BlobStoringOptions> options)
        {
            Options = options.Value;
        }

        public virtual BlobContainerConfiguration Get(string name)
        {
            return Options.Containers.GetConfiguration(name);
        }
    }
}