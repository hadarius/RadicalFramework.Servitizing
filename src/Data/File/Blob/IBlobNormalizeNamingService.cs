using Radical.Servitizing.Data.File.Blob.Container;

namespace Radical.Servitizing.Data.File.Blob
{
    public interface IBlobNormalizeNamingService
    {
        BlobNormalizeNaming NormalizeNaming(BlobContainerConfiguration configuration, string containerName, string blobName);

        string NormalizeContainerName(BlobContainerConfiguration configuration, string containerName);

        string NormalizeBlobName(BlobContainerConfiguration configuration, string blobName);
    }
}
