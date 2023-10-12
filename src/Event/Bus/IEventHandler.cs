using System.Threading.Tasks;

namespace Radical.Servitizing.Event.Bus
{
    public interface IEventHandler<in TEvent> : IEventHandler
    {
        Task HandleEventAsync(TEvent eventData);
    }

    public interface IEventHandler
    {

    }
}
