﻿using System;
using RabbitMQ.Client;

namespace Radical.Servitizing.Event.Provider.RabbitMq
{
    public interface IChannelAccessor : IDisposable
    {
        /// <summary>
        /// Reference to the channel.
        /// Never dispose the <see cref="Channel"/> object.
        /// Instead, dispose the <see cref="IChannelAccessor"/> after usage.
        /// </summary>
        IModel Channel { get; }

        /// <summary>
        /// Name of the channel.
        /// </summary>
        string Name { get; }
    }
}