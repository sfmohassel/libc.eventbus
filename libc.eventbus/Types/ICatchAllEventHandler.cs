using System.Threading.Tasks;

namespace libc.eventbus.Types
{
  /// <summary>
  ///   Implement to catch all events. When an event is raised, first its explicit handlers
  ///   are called and after that, handlers that implement this are called.
  /// </summary>
  public interface ICatchAllEventHandler
  {
    Task Handle(IEvent ev);
  }
}