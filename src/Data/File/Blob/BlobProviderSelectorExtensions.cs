﻿using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;
using Radical.Servitizing.Data.File.Blob.Container;

namespace Radical.Servitizing.Data.File.Blob
{
    public static class BlobProviderSelectorExtensions
    {
        public static IBlobProvider Get<TContainer>(
            [DisallowNull] this IBlobProviderSelector selector)
        {
            return selector.Get(BlobContainerNameAttribute.GetContainerName<TContainer>());
        }
    }
}