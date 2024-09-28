using libc.eventbus.Types;
using System;

namespace libc.eventbus.System;

/// <summary>
///     Implement to have a sink to allow other parts of application to subscribe to events
/// </summary>
public interface IEventSink : IDisposable
{
    /// <summary>
    ///     Registers catch-all event handlers
    /// </summary>
    /// <param name="handlers"></param>
    void RegisterCatchAllHandler(params ICatchAllEventHandler[] handlers);

    /// <summary>
    ///     Unregisters given catch-all event handlers
    /// </summary>
    /// <param name="handlers"></param>
    void UnregisterCatchAllHandler(params ICatchAllEventHandler[] handlers);

    /// <summary>
    ///     Subscribe your handlers to an event.
    ///     If a handler instance is equal to one defined before, this method SHOULD overwrite previous one
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TEventHandler"></typeparam>
    /// <param name="handlers"></param>
    void Subscribe<TEvent, TEventHandler>(params TEventHandler[] handlers)
        where TEvent : IEvent
        where TEventHandler : IEventHandler<TEvent>;

    /// <summary>
    ///     Unsubscribe your handlers from an event
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TEventHandler"></typeparam>
    /// <param name="handlers"></param>
    void Unsubscribe<TEvent, TEventHandler>(params TEventHandler[] handlers)
        where TEvent : IEvent
        where TEventHandler : IEventHandler<TEvent>;
}