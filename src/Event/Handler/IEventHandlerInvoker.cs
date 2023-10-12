using System;
using System.Threading.Tasks;
using Radical.Servitizing.Event.Bus;

namespace Radical.Servitizing.Event.Handler
{
    public interface IEventHandlerInvoker
    {
        Task InvokeAsync(IEventHandler eventHandler, object eventData, Type eventType);
    }
}