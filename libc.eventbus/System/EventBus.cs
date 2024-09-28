using libc.eventbus.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace libc.eventbus.System;

/// <summary>
///     An abstract event-bus that implements both <see cref="IEventEmitter" /> and <see cref="IEventSink" />
///     Implement this to have your own bus :-)
/// </summary>
public abstract class EventBus : IEventEmitter, IEventSink
{
    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="ev"></param>
    public EventPublishResult<TEvent> Publish<TEvent>(TEvent ev) where TEvent : IEvent
    {
        return PublishAsync(ev).Result;
    }

    public async Task<EventPublishResult<TEvent>> PublishAsync<TEvent>(TEvent ev) where TEvent : IEvent
    {
        var handlerResults = new List<EventPublishHandlerResult<TEvent>>();
        var catchAllResults = new List<EventPublishCatchAllHandlerResult>();

        var handlers = GetHandlers<TEvent>().ToArray();
        var catchAllHandlers = GetCatchAllHandlers().ToArray();
        var start = DateTime.UtcNow;

        foreach (var handler in handlers)
        {
            EventPublishHandlerResult<TEvent> x = null;

            try
            {
                await handler.Handle(ev);
                x = new EventPublishHandlerResult<TEvent>(handler, EventHandlerExecutionCode.Executed, null);
            }
            catch (Exception ex)
            {
                x = new EventPublishHandlerResult<TEvent>(handler, EventHandlerExecutionCode.UnhandledException,
                    ex);
            }
            finally
            {
                handlerResults.Add(x);
            }
        }

        // execute catch-all handlers
        foreach (var handler in catchAllHandlers)
        {
            EventPublishCatchAllHandlerResult x = null;

            try
            {
                await handler.Handle(ev);
                x = new EventPublishCatchAllHandlerResult(handler, EventHandlerExecutionCode.Executed, null);
            }
            catch (Exception ex)
            {
                x = new EventPublishCatchAllHandlerResult(handler, EventHandlerExecutionCode.UnhandledException,
                    ex);
            }
            finally
            {
                catchAllResults.Add(x);
            }
        }

        // store end time
        var end = DateTime.UtcNow;

        return new EventPublishResult<TEvent>(handlerResults.ToArray(), catchAllResults.ToArray(), start, end);
    }

    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TEventHandler"></typeparam>
    /// <param name="handlers"></param>
    public abstract void Subscribe<TEvent, TEventHandler>(params TEventHandler[] handlers)
        where TEvent : IEvent
        where TEventHandler : IEventHandler<TEvent>;

    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TEventHandler"></typeparam>
    /// <param name="handlers"></param>
    public abstract void Unsubscribe<TEvent, TEventHandler>(params TEventHandler[] handlers)
        where TEvent : IEvent
        where TEventHandler : IEventHandler<TEvent>;

    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <param name="handlers"></param>
    public abstract void RegisterCatchAllHandler(params ICatchAllEventHandler[] handlers);
    
    /// <summary>
    ///     <inheritdoc />
    /// </summary>
    /// <param name="handlers"></param>
    public abstract void UnregisterCatchAllHandler(params ICatchAllEventHandler[] handlers);

    /// <summary>
    ///     Use this to release resources
    /// </summary>
    public abstract void Dispose();

    /// <summary>
    ///     Get Handlers for an event type
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public abstract IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>() where TEvent : IEvent;

    /// <summary>
    ///     Get all catch-all handlers
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerable<ICatchAllEventHandler> GetCatchAllHandlers();
}