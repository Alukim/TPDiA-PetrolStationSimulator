using System.Threading.Tasks;

namespace PetrolStation.Infrastructure
{
    public interface IEventHandler<TEvent>
        where TEvent : IEvent
    {
        Task Handle(TEvent @event);
    }
}
