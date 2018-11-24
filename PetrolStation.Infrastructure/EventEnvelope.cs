using Newtonsoft.Json;

namespace PetrolStation.Infrastructure
{
    public class EventEnvelope
    {
        [JsonConstructor]
        public EventEnvelope(CallContext callContext, IEvent @event)
        {
            CallContext = callContext;
            Event = @event;
        }

        public CallContext CallContext { get; }

        public IEvent Event { get; }
    }
}
