using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PetrolStation.Infrastructure
{
    public class EventDispatcher
    {
        private readonly IServiceScopeFactory scopeFactory;
        private ILogger<EventDispatcher> logger;

        public EventDispatcher(IServiceScopeFactory scopeFactory, ILogger<EventDispatcher> logger)
        {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
        }

        public async Task<bool> Dispatch(EventEnvelope eventEnvelope)
        {
            try
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    scope.ServiceProvider.GetRequiredService<CallContext>().InitializeCallContextFrom(eventEnvelope.CallContext);

                    var eventBus = scope.ServiceProvider.GetRequiredService<EventBus>();

                    await eventBus.PublishEventAsync(eventEnvelope.Event);
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error during event processing");
                return false;
            }

            return true;
        }
    }
}
