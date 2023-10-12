using System;
using Radical.Servitizing.Event.Bus;

namespace Radical.Servitizing.Event.Handler
{
    public class EventHandlerDisposeWrapper : IEventHandlerDisposeWrapper
    {
        public IEventHandler EventHandler { get; }

        private readonly Action _disposeAction;

        public EventHandlerDisposeWrapper(IEventHandler eventHandler, Action disposeAction = null)
        {
            _disposeAction = disposeAction;
            EventHandler = eventHandler;
        }

        public void Dispose()
        {
            _disposeAction?.Invoke();
        }
    }
}