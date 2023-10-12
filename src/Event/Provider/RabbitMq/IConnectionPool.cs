using System;
using RabbitMQ.Client;

namespace Radical.Servitizing.Event.Provider.RabbitMq
{
    public interface IConnectionPool : IDisposable
    {
        IConnection Get(string connectionName = null);
    }
}