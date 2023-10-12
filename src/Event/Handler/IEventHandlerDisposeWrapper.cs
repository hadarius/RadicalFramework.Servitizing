using System;
using Radical.Servitizing.Event.Bus;

namespace Radical.Servitizing.Event.Handler
{
    public interface IEventHandlerDisposeWrapper : IDisposable
    {
        IEventHandler EventHandler { get; }
    }
}
