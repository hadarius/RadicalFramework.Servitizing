using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Radical.Logging;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Azure.Cosmos.Linq;
using NLog.Fluent;
using Radical.Servitizing.Data.File.Blob.Container;

namespace Radical.Servitizing.Data.File.Blob
{
    public class BlobProviderSaveArgs : BlobProviderArgs
    {
        [DisallowNull]
        public Stream BlobStream { get; }

        public bool OverrideExisting { get; }

        public BlobProviderSaveArgs(
            [DisallowNull] string containerName,
            [DisallowNull] BlobContainerConfiguration configuration,
            [DisallowNull] string blobName,
            [DisallowNull] Stream blobStream,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default)
            : base(
                containerName,
                configuration,
                blobName,
                cancellationToken)
        {
            if (blobStream.IsNull()) { this.Warning<Runlog, Exception>("stream is null"); }
            BlobStream = blobStream;
            OverrideExisting = overrideExisting;
        }
    }
}
