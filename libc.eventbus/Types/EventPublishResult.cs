using System;

namespace libc.eventbus.Types
{
  /// <summary>
  ///     Result of publishing an <see cref="IEvent" />
  /// </summary>
  public class EventPublishResult<TEvent> where TEvent : IEvent
  {
    public EventPublishResult(EventPublishHandlerResult<TEvent>[] handlerResults,
        EventPublishCatchAllHandlerResult[] catchAllResults, DateTime startedAt, DateTime finishedAt)
    {
      HandlerResults = handlerResults;
      CatchAllResults = catchAllResults;
      StartedAt = startedAt;
      FinishedAt = finishedAt;
    }

    /// <summary>
    ///     Array of handler results
    /// </summary>
    public EventPublishHandlerResult<TEvent>[] HandlerResults { get; private set; }

    /// <summary>
    ///     Array of catch-all handler results
    /// </summary>
    public EventPublishCatchAllHandlerResult[] CatchAllResults { get; private set; }

    /// <summary>
    ///     The timestamp just before first execution. In UTC
    /// </summary>
    public DateTime StartedAt { get; private set; }

    /// <summary>
    ///     The timestamp just after last execution. In UTC
    /// </summary>
    public DateTime FinishedAt { get; private set; }

    /// <summary>
    ///     Duration of executing handlers
    /// </summary>
    public TimeSpan Duration => FinishedAt.Subtract(StartedAt);
  }

  public class EventPublishHandlerResult<TEvent> where TEvent : IEvent
  {
    public EventPublishHandlerResult(IEventHandler<TEvent> handler,
        EventHandlerExecutionCode executionCode, Exception error)
    {
      Handler = handler;
      ExecutionCode = executionCode;
      Error = error;
    }

    public IEventHandler<TEvent> Handler { get; private set; }
    public EventHandlerExecutionCode ExecutionCode { get; private set; }
    public Exception Error { get; private set; }
  }

  public class EventPublishCatchAllHandlerResult
  {
    public EventPublishCatchAllHandlerResult(ICatchAllEventHandler handler,
        EventHandlerExecutionCode executionCode, Exception error)
    {
      Handler = handler;
      ExecutionCode = executionCode;
      Error = error;
    }

    public ICatchAllEventHandler Handler { get; private set; }
    public EventHandlerExecutionCode ExecutionCode { get; private set; }
    public Exception Error { get; private set; }
  }
}