using Radical.Servitizing.Data.File.Blob.Container;

namespace Radical.Servitizing.Data.File.Blob
{
    public class BlobStoringOptions
    {
        public BlobContainerConfigurations Containers { get; }

        public BlobStoringOptions()
        {
            Containers = new BlobContainerConfigurations();
        }
    }
}