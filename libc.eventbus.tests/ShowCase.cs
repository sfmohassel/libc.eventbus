using System;
using System.Threading.Tasks;
using libc.eventbus.System;
using libc.eventbus.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace libc.eventbus.tests;

[TestClass]
public class Showcase
{
  [TestMethod]
  public void Showcase_WithoutCatchAll()
  {
    // 1- create an event bus
    var bus = new DefaultEventBus();

    // 2- subscribe to SimpleMessage event via PrintMessageRaw event handler
    bus.Subscribe<SimpleMessage, PrintMessageRaw>(new PrintMessageRaw());

    // 3- subscribe to SimpleMessage event via PrintMessagePretty event handler
    var x = new PrintMessagePretty();
    bus.Subscribe<SimpleMessage, PrintMessagePretty>(x);

    // 4- remember subscribing to a message with the same handler instance, has no effect!
    bus.Subscribe<SimpleMessage, PrintMessagePretty>(x);

    // 5- create the event
    var message = new SimpleMessage("a simple message");

    // 6- publish the event
    bus.Publish(message);
  }

  [TestMethod]
  public void Showcase_WithCatchAll()
  {
    // 1- create an event bus
    var bus = new DefaultEventBus();

    // 2- subscribe to SimpleMessage event via PrintMessageRaw event handler
    bus.Subscribe<SimpleMessage, PrintMessageRaw>(new PrintMessageRaw());

    // 3- subscribe to SimpleMessage event via PrintMessagePretty event handler
    bus.Subscribe<SimpleMessage, PrintMessagePretty>(new PrintMessagePretty());

    // 4- register a catch-all event handler
    bus.RegisterCatchAllHandler(new CatchAllMessages());

    // 5- create the event
    var message = new SimpleMessage("a simple message");

    // 6- publish the event
    bus.Publish(message);
  }

  public class SimpleMessage : IEvent
  {
    public SimpleMessage(string text)
    {
      Text = text;
    }

    public string Text { get; }
  }

  public class PrintMessageRaw : IEventHandler<SimpleMessage>
  {
    public Task Handle(SimpleMessage ev)
    {
      // print message
      Console.WriteLine($"Raw: {ev.Text}");

      return Task.CompletedTask;
    }
  }

  public class PrintMessagePretty : IEventHandler<SimpleMessage>
  {
    public Task Handle(SimpleMessage ev)
    {
      // print message
      Console.WriteLine($"Pretty: {ev.Text}");

      return Task.CompletedTask;
    }
  }

  public class PrivateMessage : IEvent
  {
    public PrivateMessage(string secret)
    {
      Secret = secret;
    }

    public string Secret { get; private set; }
  }

  public class CatchAllMessages : ICatchAllEventHandler
  {
    public Task Handle(IEvent ev)
    {
      if (ev is SimpleMessage)
        Console.WriteLine($"Caught SimpleMessage: {(ev as SimpleMessage).Text}");
      else if (ev is PrivateMessage)
        Console.WriteLine($"Caught PrivateMessage: {(ev as PrivateMessage).Secret}");

      return Task.CompletedTask;
    }
  }
}