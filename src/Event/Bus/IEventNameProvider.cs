using System;

namespace Radical.Servitizing.Event.Bus
{
    public interface IEventNameProvider
    {
        string GetName(Type eventType);
    }
}