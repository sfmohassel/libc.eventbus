using System;
using System.Collections.Generic;
using System.Linq;
using libc.eventbus.Types;

namespace libc.eventbus.System
{
  /// <summary>
  ///   A default implementation for <see cref="EventBus" /> which uses an internal cache for
  ///   subscriptions
  /// </summary>
  public class DefaultEventBus : EventBus
  {
    private readonly Cache _cache = new Cache();

    /// <summary>
    ///   <inheritdoc />
    /// </summary>
    /// <param name="handlers"></param>
    public override void RegisterCatchAllHandler(IEnumerable<ICatchAllEventHandler> handlers)
    {
      if (handlers == null) return;

      foreach (var handler in handlers) _cache.Add(handler);
    }

    /// <summary>
    ///   <inheritdoc />
    /// </summary>
    /// <param name="handlers"></param>
    public override void UnregisterCatchAllHandler(IEnumerable<ICatchAllEventHandler> handlers)
    {
      if (handlers == null) return;

      foreach (var handler in handlers) _cache.Remove(handler);
    }

    /// <summary>
    ///   <inheritdoc />
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TEventHandler"></typeparam>
    /// <param name="handlers"></param>
    public override void Subscribe<TEvent, TEventHandler>(IEnumerable<TEventHandler> handlers)
    {
      if (handlers == null) return;

      foreach (var handler in handlers) _cache.Add(handler);
    }

    /// <summary>
    ///   <inheritdoc />
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TEventHandler"></typeparam>
    /// <param name="handlers"></param>
    public override void Unsubscribe<TEvent, TEventHandler>(IEnumerable<TEventHandler> handlers)
    {
      if (handlers == null) return;

      foreach (var handler in handlers) _cache.Remove(handler);
    }

    /// <summary>
    ///   <inheritdoc />
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<ICatchAllEventHandler> GetCatchAllHandlers()
    {
      return _cache.GetAllCatchAllHandlers();
    }

    /// <summary>
    ///   <inheritdoc />
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public override IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>()
    {
      return _cache.GetHandlers<TEvent>();
    }

    /// <summary>
    ///   <inheritdoc />
    /// </summary>
    public override void Dispose()
    {
      _cache?.Dispose();
    }

    private class Cache : IDisposable
    {
      private volatile bool _disposing, _disposed;

      public Cache()
      {
        CatchAllEventHandlers = new Dictionary<ICatchAllEventHandler, int>();
        Handlers = new Dictionary<Type, IDictionary<object, int>>();
      }

      private IDictionary<ICatchAllEventHandler, int> CatchAllEventHandlers { get; }
      private IDictionary<Type, IDictionary<object, int>> Handlers { get; }

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

        if (!Handlers.TryGetValue(type, out var handle)) return;
        handle.Remove(handler);
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

        return !Handlers.TryGetValue(type, out var handler)
          ? Array.Empty<IEventHandler<TEvent>>()
          : handler.Keys.OfType<IEventHandler<TEvent>>();
      }
    }
  }
}