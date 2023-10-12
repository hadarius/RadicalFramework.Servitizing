﻿using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Radical.Servitizing.Data.File.Blob
{
    public interface IBlobContainer<TContainer> : IBlobContainer
        where TContainer : class
    {

    }

    public interface IBlobContainer
    {
        Task SaveAsync(
            string name,
            Stream stream,
            bool overrideExisting = false,
            CancellationToken cancellationToken = default
        );

        Task<bool> DeleteAsync(
            string name,
            CancellationToken cancellationToken = default
        );

        Task<bool> ExistsAsync(
            string name,
            CancellationToken cancellationToken = default
        );

        Task<Stream> GetAsync(
            string name,
            CancellationToken cancellationToken = default
        );

        Task<Stream> GetOrNullAsync(
            string name,
            CancellationToken cancellationToken = default
        );

    }
}