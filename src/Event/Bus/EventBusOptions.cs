
using System.Collections.Generic;

namespace Radical.Servitizing.Event.Bus
{
    public class EventBusOptions
    {
        public IList<IEventHandler> Handlers { get; }

        public EventBusOptions()
        {
            Handlers = new List<IEventHandler>();
        }
    }
}
