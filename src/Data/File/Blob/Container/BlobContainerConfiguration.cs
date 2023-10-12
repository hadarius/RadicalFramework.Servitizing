using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Radical.Series;
using JetBrains.Annotations;

namespace Radical.Servitizing.Data.File.Blob.Container
{
    public class BlobContainerConfiguration
    {
        /// <summary>
        /// The provider to be used to store BLOBs of this container.
        /// </summary>
        public Type ProviderType { get; set; }

        public IList<IBlobNamingNormalizer> NamingNormalizers { get; }

        [DisallowNull] private readonly Registry<object> _properties;

        [AllowNull] private readonly BlobContainerConfiguration _fallbackConfiguration;

        public BlobContainerConfiguration(BlobContainerConfiguration fallbackConfiguration = null)
        {
            NamingNormalizers = new List<IBlobNamingNormalizer>();
            _fallbackConfiguration = fallbackConfiguration;
            _properties = new Registry<object>();
        }

        public T GetConfigurationOrDefault<T>(string name, T defaultValue = default)
        {
            return (T)GetConfigurationOrNull(name, defaultValue);
        }

        public object GetConfigurationOrNull(string name, object defaultValue = null)
        {
            return _properties.Get(name) ??
                   _fallbackConfiguration?.GetConfigurationOrNull(name, defaultValue) ??
                   defaultValue;
        }

        public BlobContainerConfiguration SetConfiguration([DisallowNull] string name, [AllowNull] object value)
        {
            _properties[name] = value;

            return this;
        }


        public BlobContainerConfiguration ClearConfiguration([DisallowNull] string name)
        {

            _properties.Remove(name);

            return this;
        }
    }
}