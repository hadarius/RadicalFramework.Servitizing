using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace Radical.Servitizing.Data.File.Blob
{
    public interface IBlobProviderSelector
    {
        IBlobProvider Get([DisallowNull] string containerName);
    }
}