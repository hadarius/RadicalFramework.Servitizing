﻿using System.IO;

namespace Radical.Servitizing.Data.File.Blob.FileSystem
{
    public class DefaultBlobFilePathCalculator : IBlobFilePathCalculator
    {
        public DefaultBlobFilePathCalculator()
        {
        }

        public virtual string Calculate(BlobProviderArgs args)
        {
            var fileSystemConfiguration = args.Configuration.GetFileSystemConfiguration();
            var blobPath = fileSystemConfiguration.BasePath;

            if (fileSystemConfiguration.AppendContainerNameToBasePath)
            {
                blobPath = Path.Combine(blobPath, args.ContainerName);
            }

            blobPath = Path.Combine(blobPath, args.BlobName);

            return blobPath;
        }
    }
}