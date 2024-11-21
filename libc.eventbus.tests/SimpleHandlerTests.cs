using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using libc.eventbus.System;
using libc.eventbus.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace libc.eventbus.tests;

[TestClass]
public class SimpleHandlerTests
{
  [TestMethod]
  public void OneHandler_OneEvent()
  {
    var bus = new DefaultEventBus();
    var logStream = new List<LogEvent>();
    var handler = new LogEventHandler1(logStream);
    bus.Subscribe<LogEvent, LogEventHandler1>(handler);
    var ev = new LogEvent("log-1");
    var res = bus.Publish(ev);

    Assert.IsTrue(logStream.Count == 1,
      "There must be one log item in stream");

    Assert.IsTrue(logStream[0].Text.Equals(ev.Text, StringComparison.Ordinal),
      "Log item's text aren't equal");

    Assert.IsNotNull(res,
      "Event handler was not found");

    Assert.IsNotNull(res.CatchAllResults,
      "Catch-all handlers must not be null");

    Assert.IsTrue(res.CatchAllResults.Length == 0,
      "There must be no catch-all results");

    Assert.IsTrue(res.StartedAt <= res.FinishedAt,
      "StartedAt > FinishedAt!");

    Assert.IsTrue(res.Duration.Ticks >= 0,
      "Duration is negative");

    Assert.IsNotNull(res.HandlerResults,
      "Handler result is null");

    Assert.IsTrue(res.HandlerResults.Length == 1,
      "Handler result is not added");

    Assert.IsTrue(res.HandlerResults[0].Handler.Equals(handler),
      "Handler in result is not equal to the actual handler!");

    bus.Dispose();
  }

  [TestMethod]
  public void MultipleHandlers_OneEvent()
  {
    var bus = new DefaultEventBus();
    var logStream1 = new List<LogEvent>();
    var logStream2 = new List<LogEvent>();
    var handler1 = new LogEventHandler1(logStream1);
    var handler2 = new LogEventHandler2(logStream2);
    bus.Subscribe<LogEvent, LogEventHandler1>(handler1);
    bus.Subscribe<LogEvent, LogEventHandler2>(handler2);
    var ev = new LogEvent("log");
    var res = bus.Publish(ev);

    Assert.IsTrue(logStream1.Count == 1,
      "There must be one log item in stream1");

    Assert.IsTrue(logStream1[0].Text.Equals(ev.Text, StringComparison.Ordinal),
      "Log item's text aren't equal in stream1");

    Assert.IsTrue(logStream2.Count == 1,
      "There must be one log item in stream2");

    Assert.IsTrue(logStream2[0].Text.Equals(ev.Text, StringComparison.Ordinal),
      "Log item's text aren't equal in stream2");

    Assert.IsNotNull(res,
      "Event handler was not found");

    Assert.IsNotNull(res.CatchAllResults,
      "Catch-all handlers must not be null");

    Assert.IsTrue(res.CatchAllResults.Length == 0,
      "There must be no catch-all results");

    Assert.IsTrue(res.StartedAt <= res.FinishedAt,
      "StartedAt > FinishedAt!");

    Assert.IsTrue(res.Duration.Ticks >= 0,
      "Duration is negative");

    Assert.IsNotNull(res.HandlerResults,
      "Handler result is null");

    Assert.IsTrue(res.HandlerResults.Length == 2,
      "Handler result is not added");

    Assert.IsTrue(res.HandlerResults.Any(a => a.Handler.Equals(handler1)),
      "handler1 does not exist in result");

    Assert.IsTrue(res.HandlerResults.Any(a => a.Handler.Equals(handler2)),
      "handler2 does not exist in result");

    bus.Dispose();
  }

  [TestMethod]
  public void OneHandler_OneEvent_OneCatchAll()
  {
    var bus = new DefaultEventBus();
    var logStream = new List<LogEvent>();
    var someStream = new List<SomeEvent>();
    var handler = new LogEventHandler1(logStream);
    bus.Subscribe<LogEvent, LogEventHandler1>(handler);
    var ev = new LogEvent("log-1");
    var res = bus.Publish(ev);

    Assert.IsTrue(logStream.Count == 1,
      "There must be one log item in stream");

    Assert.IsTrue(logStream[0].Text.Equals(ev.Text, StringComparison.Ordinal),
      "Log item's text aren't equal");

    Assert.IsNotNull(res,
      "Event handler was not found");

    Assert.IsNotNull(res.CatchAllResults,
      "Catch-all handlers must not be null");

    Assert.IsTrue(res.CatchAllResults.Length == 0,
      "There must 1 catch-all result");

    Assert.IsTrue(res.StartedAt <= res.FinishedAt,
      "StartedAt > FinishedAt!");

    Assert.IsTrue(res.Duration.Ticks >= 0,
      "Duration is negative");

    Assert.IsNotNull(res.HandlerResults,
      "Handler result is null");

    Assert.IsTrue(res.HandlerResults.Length == 1,
      "Handler result is not added");

    Assert.IsTrue(res.HandlerResults[0].Handler.Equals(handler),
      "Handler in result is not equal to the actual handler!");

    var catchAllHandler = new CatchAllHandler(logStream, someStream);
    bus.RegisterCatchAllHandler(catchAllHandler);
    var ev2 = new SomeEvent(100);
    var res2 = bus.Publish(ev2);

    Assert.IsTrue(someStream.Count == 1,
      "There must be one some event in stream");

    Assert.IsTrue(someStream[0].Number == ev2.Number,
      "some event number wrong");

    Assert.IsNotNull(res2,
      "Event handler was not found");

    Assert.IsNotNull(res2.CatchAllResults,
      "Catch-all handlers must not be null");

    Assert.IsTrue(res2.CatchAllResults.Length == 1,
      "There must be 1 catch-all result");

    Assert.IsTrue(res2.StartedAt <= res2.FinishedAt,
      "StartedAt > FinishedAt!");

    Assert.IsTrue(res2.Duration.Ticks >= 0,
      "Duration is negative");

    Assert.IsNotNull(res2.HandlerResults,
      "Handler result is null");

    Assert.IsTrue(res2.HandlerResults.Length == 0,
      "Handler result is not added");

    Assert.IsTrue(res2.CatchAllResults[0].Handler.Equals(catchAllHandler),
      "Catch-all Handler in result is not equal to the actual handler!");

    bus.Dispose();
  }

  public class LogEvent : IEvent
  {
    public LogEvent(string text)
    {
      Text = text;
    }

    public string Text { get; private set; }
  }

  public class SomeEvent : IEvent
  {
    public SomeEvent(int number)
    {
      Number = number;
    }

    public int Number { get; private set; }
  }

  public class LogEventHandler1 : IEventHandler<LogEvent>
  {
    private readonly List<LogEvent> logStream;

    public LogEventHandler1(List<LogEvent> logStream)
    {
      this.logStream = logStream;
    }

    public Task Handle(LogEvent ev)
    {
      logStream.Add(ev);

      return Task.CompletedTask;
    }
  }

  public class LogEventHandler2 : IEventHandler<LogEvent>
  {
    private readonly List<LogEvent> logStream;

    public LogEventHandler2(List<LogEvent> logStream)
    {
      this.logStream = logStream;
    }

    public Task Handle(LogEvent ev)
    {
      logStream.Add(ev);

      return Task.CompletedTask;
    }
  }

  public class CatchAllHandler : ICatchAllEventHandler
  {
    private readonly List<LogEvent> logStream;
    private readonly List<SomeEvent> someStream;

    public CatchAllHandler(List<LogEvent> logStream, List<SomeEvent> someStream)
    {
      this.logStream = logStream;
      this.someStream = someStream;
    }

    public Task Handle(IEvent ev)
    {
      if (ev is LogEvent)
        logStream.Add(ev as LogEvent);
      else if (ev is SomeEvent) someStream.Add(ev as SomeEvent);

      return Task.CompletedTask;
    }
  }
}