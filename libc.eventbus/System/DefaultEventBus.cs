using libc.eventbus.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace libc.eventbus.System {
    /// <summary>
    /// A default implementation for <see cref="EventBus"/> which uses an internal cache for subscriptions
    /// </summary>
    public class DefaultEventBus : EventBus {

        private readonly Cache cache;

        public DefaultEventBus() {
            cache = new Cache();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="handlers"></param>
        public override void RegisterCatchAllHandler(IEnumerable<ICatchAllEventHandler> handlers) {
            if (handlers == null) {
                return;
            }
            foreach (var handler in handlers) {
                cache.Add(handler);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="handlers"></param>
        public override void UnregisterCatchAllHandler(IEnumerable<ICatchAllEventHandler> handlers) {
            if (handlers == null) {
                return;
            }
            foreach (var handler in handlers) {
                cache.Remove(handler);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        /// <param name="handlers"></param>
        public override void Subscribe<TEvent, TEventHandler>(IEnumerable<TEventHandler> handlers) {
            if (handlers == null) {
                return;
            }
            foreach (var handler in handlers) {
                cache.Add(handler);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        /// <param name="handlers"></param>
        public override void Unsubscribe<TEvent, TEventHandler>(IEnumerable<TEventHandler> handlers) {
            if (handlers == null) {
                return;
            }
            foreach (var handler in handlers) {
                cache.Remove(handler);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ICatchAllEventHandler> GetCatchAllHandlers() {
            return cache.GetAllCatchAllHandlers();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public override IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>() {
            return cache.GetHandlers<TEvent>();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Dispose() {
            cache?.Dispose();
        }

        private class Cache : IDisposable {
            private volatile bool disposing, disposed;
            private IDictionary<ICatchAllEventHandler, int> CatchAllEventHandlers { get; }
            private IDictionary<Type, IDictionary<object, int>> Handlers { get; }

            public Cache() {
                CatchAllEventHandlers = new Dictionary<ICatchAllEventHandler, int>();
                Handlers = new Dictionary<Type, IDictionary<object, int>>();
            }

            public void Add(ICatchAllEventHandler handler) {
                if (disposing || disposed) {
                    return;
                }
                CatchAllEventHandlers[handler] = 0;
            }

            public void Add<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent {
                if (disposing || disposed) {
                    return;
                }
                var type = typeof(TEvent);
                if (!Handlers.ContainsKey(type)) {
                    Handlers[type] = new Dictionary<object, int>();
                }
                Handlers[type][handler] = 0;
            }

            public void Remove(ICatchAllEventHandler handler) {
                if (disposing || disposed) {
                    return;
                }
                CatchAllEventHandlers.Remove(handler);
            }

            public void Remove<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent {
                if (disposing || disposed) {
                    return;
                }
                var type = typeof(TEvent);
                if (!Handlers.ContainsKey(type)) {
                    return;
                }
                Handlers[type].Remove(handler);
            }

            public IEnumerable<ICatchAllEventHandler> GetAllCatchAllHandlers() {
                if (disposing || disposed) {
                    return new ICatchAllEventHandler[0];
                }
                return CatchAllEventHandlers.Keys;
            }

            public IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>() where TEvent : IEvent {
                if (disposing || disposed) {
                    return new IEventHandler<TEvent>[0];
                }
                var type = typeof(TEvent);
                if (!Handlers.ContainsKey(type)) {
                    return new IEventHandler<TEvent>[0];
                }
                return Handlers[type].Keys.OfType<IEventHandler<TEvent>>();
            }

            public void Dispose() {
                try {
                    disposing = true;
                    CatchAllEventHandlers?.Clear();
                    foreach (var dic in Handlers.Values) {
                        dic?.Clear();
                    }
                    Handlers?.Clear();
                } catch {
                } finally {
                    disposed = true;
                }
            }
        }
    }
}
