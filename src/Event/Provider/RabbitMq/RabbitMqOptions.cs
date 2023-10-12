namespace Radical.Servitizing.Event.Provider.RabbitMq
{
    public class RabbitMqOptions
    {
        public RabbitMqConnections Connections { get; }

        public RabbitMqOptions()
        {
            Connections = new RabbitMqConnections();
        }
    }
}