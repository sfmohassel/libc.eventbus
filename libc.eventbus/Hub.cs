using System;
using System.Collections;
using System.Collections.Generic;
using Autofac;
namespace libc.eventbus {
    public sealed class Hub : IHub {
        private readonly ILifetimeScope container;
        public Hub(ILifetimeScope container) {
            this.container = container;
        }
        public void Publish<TEvent>(TEvent ev) where TEvent : IEvent {
            var evType = ev.GetType();
            //handlers
            var handlers = getHandlers(evType);
            foreach (dynamic handler in handlers) handler.Handle((dynamic) ev);
            //catch all handlers
            foreach (var catchAllEventHandler in getCatchAllHandlers()) catchAllEventHandler.Handle(ev);
        }
        private IEnumerable<ICatchAllEventHandler> getCatchAllHandlers() {
            return container.Resolve<IEnumerable<ICatchAllEventHandler>>();
        }
        private IEnumerable getHandlers(Type evType) {
            return (IEnumerable) container.Resolve(
                typeof(IEnumerable<>).MakeGenericType(
                    typeof(IEventHandler<>).MakeGenericType(evType)
                )
            );
        }
    }
}