﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Radical.Series;
using Radical.Uniques;
using JetBrains.Annotations;

namespace Radical.Servitizing.Data.File.Blob.Container
{
    public class BlobContainerConfigurations
    {
        private BlobContainerConfiguration Default => GetConfiguration<DefaultContainer>();

        private readonly Registry<BlobContainerConfiguration> _containers;

        public BlobContainerConfigurations()
        {
            _containers = new Registry<BlobContainerConfiguration>(true)
            {
                //Add default container
                [BlobContainerNameAttribute.GetContainerName<DefaultContainer>()] = new BlobContainerConfiguration()
            };
        }

        public BlobContainerConfigurations Configure<TContainer>(
            Action<BlobContainerConfiguration> configureAction)
        {
            return Configure(
                BlobContainerNameAttribute.GetContainerName<TContainer>(),
                configureAction
            );
        }

        public BlobContainerConfigurations Configure(
            [DisallowNull] string name,
            [DisallowNull] Action<BlobContainerConfiguration> configureAction)
        {

            configureAction(
                _containers.EnsureGet(
                    name.UniqueKey(),
                    (a) => new BlobContainerConfiguration(Default)
                ).Value
            );

            return this;
        }

        public BlobContainerConfigurations ConfigureDefault(Action<BlobContainerConfiguration> configureAction)
        {
            configureAction(Default);
            return this;
        }

        public BlobContainerConfigurations ConfigureAll(Action<ulong, BlobContainerConfiguration> configureAction)
        {
            foreach (var container in _containers)
            {
                configureAction(container.Key, container.Value);
            }

            return this;
        }

        public BlobContainerConfiguration GetConfiguration<TContainer>()
        {
            return GetConfiguration(BlobContainerNameAttribute.GetContainerName<TContainer>());
        }

        public BlobContainerConfiguration GetConfiguration([DisallowNull] string name)
        {

            return _containers.Get(name) ??
                   Default;
        }
    }
}