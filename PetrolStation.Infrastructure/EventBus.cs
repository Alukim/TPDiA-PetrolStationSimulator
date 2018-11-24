using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Infrastructure
{
    public class EventBus
    {
        private readonly IServiceProvider serviceProvider;

        public EventBus(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task PublishEventAsync(IEvent @event)
        {
            var genericHandlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
            var genericHandlerWrapperType = typeof(AsyncEventHandlerWrapper<>).MakeGenericType(@event.GetType());

            var handlers = serviceProvider
                .GetServices(genericHandlerType)
                .Select(x => Activator.CreateInstance(genericHandlerWrapperType, x))
                .Cast<AsyncEventHandlerWrapper>()
                .ToList();

            foreach(var handler in handlers)
            {
                await handler.Handle(@event);
            }
        }

        private abstract class AsyncEventHandlerWrapper
        {
            public abstract Task Handle(IEvent @event);
        }

        private class AsyncEventHandlerWrapper<TEvent> : AsyncEventHandlerWrapper
            where TEvent : IEvent
        {
            private readonly IEventHandler<TEvent> genericHandler;

            public AsyncEventHandlerWrapper(IEventHandler<TEvent> inner) => genericHandler = inner;

            public override Task Handle(IEvent @event) => genericHandler.Handle((TEvent)@event);
        }
    }
}
