using System;
using System.Collections.Generic;
using System.Linq;
using Radical.Servitizing.Event.Bus;

namespace Radical.Servitizing.Event.Handler
{
    public class EventHandlerSingletonFactory : IEventHandlerFactory
    {
        public Type HandlerType => HandlerInstance.GetType();

        public IEventHandler HandlerInstance { get; }

        public EventHandlerSingletonFactory(IEventHandler handler)
        {
            HandlerInstance = handler;
        }

        public IEventHandlerDisposeWrapper GetHandler()
        {
            return new EventHandlerDisposeWrapper(HandlerInstance);
        }

        public bool IsInFactories(List<IEventHandlerFactory> handlerFactories)
        {
            return handlerFactories
                .OfType<EventHandlerSingletonFactory>()
                .Any(f => f.HandlerInstance == HandlerInstance);
        }
    }
}
