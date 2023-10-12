using System;
using Radical.Servitizing.Event.Bus;

namespace Radical.Servitizing.Event.Handler
{
    public class EventHandlerFactoryUnregistrar : IDisposable
    {
        private readonly IEventBus _eventBus;
        private readonly Type _eventType;
        private readonly IEventHandlerFactory _factory;

        public EventHandlerFactoryUnregistrar(IEventBus eventBus, Type eventType, IEventHandlerFactory factory)
        {
            _eventBus = eventBus;
            _eventType = eventType;
            _factory = factory;
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe(_eventType, _factory);
        }
    }
}