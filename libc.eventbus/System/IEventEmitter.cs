using System.Threading.Tasks;
using libc.eventbus.Types;

namespace libc.eventbus.System
{
  /// <summary>
  ///   Implement to allow other parts of application to raise events
  /// </summary>
  public interface IEventEmitter
  {
    /// <summary>
    ///   Raises an event
    /// </summary>
    /// <typeparam name="TEvent">Type of event object</typeparam>
    /// <param name="ev">Event instance</param>
    /// <returns>
    ///   <see cref="EventPublishResult{TEvent}" />
    /// </returns>
    EventPublishResult<TEvent> Publish<TEvent>(TEvent ev) where TEvent : IEvent;

    /// <summary>
    ///   Raises an event asynchronous
    /// </summary>
    /// <typeparam name="TEvent">Type of event object</typeparam>
    /// <param name="ev">Event instance</param>
    /// <returns>
    ///   <see cref="Task{EventPublishResult{TEvent}" />
    /// </returns>
    Task<EventPublishResult<TEvent>> PublishAsync<TEvent>(TEvent ev) where TEvent : IEvent;
  }
}