using System;
using System.Threading.Tasks;
using Radical.Servitizing.Event.Bus;

namespace Radical.Servitizing.Event.Handler
{
    public class ActionEventHandler<TEvent> : IEventHandler<TEvent>
    {
        public Func<TEvent, Task> Action { get; }

        public ActionEventHandler(Func<TEvent, Task> handler)
        {
            Action = handler;
        }

        public async Task HandleEventAsync(TEvent eventData)
        {
            await Action(eventData);
        }
    }
}