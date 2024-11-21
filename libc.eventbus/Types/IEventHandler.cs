using System.Threading.Tasks;

namespace libc.eventbus.Types
{
  /// <summary>
  ///   To handle an event, create a class implementing this
  /// </summary>
  /// <typeparam name="TEvent">Type of event that this handler will be notified of</typeparam>
  public interface IEventHandler<in TEvent> where TEvent : IEvent
  {
    /// <summary>
    ///   When an event of type TEvent is raised this method is called
    /// </summary>
    /// <param name="ev">Event instance</param>
    Task Handle(TEvent ev);
  }
}