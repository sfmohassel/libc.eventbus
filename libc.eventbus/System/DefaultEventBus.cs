using libc.eventbus.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace libc.eventbus.System;

/// <summary>
///     A default implementation for <see cref="EventBus" /> which uses an internal cache for subscriptions
/// </summary>
public class DefaultEventBus : EventBus
{
    private readonly Cache _cache = new();

    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <param name="handlers"></param>
    public override void RegisterCatchAllHandler(params ICatchAllEventHandler[] handlers)
    {
        if (handlers == null) return;

        foreach (var handler in handlers) _cache.Add(handler);
    }

    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <param name="handlers"></param>
    public override void UnregisterCatchAllHandler(params ICatchAllEventHandler[] handlers)
    {
        if (handlers == null) return;

        foreach (var handler in handlers) _cache.Remove(handler);
    }

    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TEventHandler"></typeparam>
    /// <param name="handlers"></param>
    public override void Subscribe<TEvent, TEventHandler>(params TEventHandler[] handlers)
    {
        if (handlers == null) return;

        foreach (var handler in handlers) _cache.Add(handler);
    }

    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TEventHandler"></typeparam>
    /// <param name="handlers"></param>
    public override void Unsubscribe<TEvent, TEventHandler>(params TEventHandler[] handlers)
    {
        if (handlers == null) return;

        foreach (var handler in handlers) _cache.Remove(handler);
    }

    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<ICatchAllEventHandler> GetCatchAllHandlers()
    {
        return _cache.GetAllCatchAllHandlers();
    }

    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public override IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>()
    {
        return _cache.GetHandlers<TEvent>();
    }

    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    public override void Dispose()
    {
        _cache?.Dispose();
    }

    private class Cache : IDisposable
    {
        private volatile bool _disposing, _disposed;

        private IDictionary<ICatchAllEventHandler, int> CatchAllEventHandlers { get; } =
            new Dictionary<ICatchAllEventHandler, int>();

        private IDictionary<Type, IDictionary<object, int>> Handlers { get; } =
            new Dictionary<Type, IDictionary<object, int>>();

        public void Dispose()
        {
            try
            {
                _disposing = true;
                CatchAllEventHandlers?.Clear();
                foreach (var dic in Handlers.Values) dic?.Clear();
                Handlers?.Clear();
            }
            catch
            {
                // ignored
            }
            finally
            {
                _disposed = true;
            }
        }

        public void Add(ICatchAllEventHandler handler)
        {
            if (_disposing || _disposed) return;
            CatchAllEventHandlers[handler] = 0;
        }

        public void Add<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
        {
            if (_disposing || _disposed) return;
            var type = typeof(TEvent);
            if (!Handlers.ContainsKey(type)) Handlers[type] = new Dictionary<object, int>();
            Handlers[type][handler] = 0;
        }

        public void Remove(ICatchAllEventHandler handler)
        {
            if (_disposing || _disposed) return;
            CatchAllEventHandlers.Remove(handler);
        }

        public void Remove<TEvent>(IEventHandler<TEvent> handler) where TEvent : IEvent
        {
            if (_disposing || _disposed) return;
            var type = typeof(TEvent);

            if (Handlers.TryGetValue(type, out var handlerToRemove))
                handlerToRemove.Remove(handler);
        }

        public IEnumerable<ICatchAllEventHandler> GetAllCatchAllHandlers()
        {
            if (_disposing || _disposed) return Array.Empty<ICatchAllEventHandler>();

            return CatchAllEventHandlers.Keys;
        }

        public IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>() where TEvent : IEvent
        {
            if (_disposing || _disposed) return Array.Empty<IEventHandler<TEvent>>();
            var type = typeof(TEvent);

            return Handlers.TryGetValue(type, out var handler)
                ? handler.Keys.OfType<IEventHandler<TEvent>>()
                : Array.Empty<IEventHandler<TEvent>>();
        }
    }
}