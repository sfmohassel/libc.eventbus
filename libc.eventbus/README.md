# An in-memory event bus without any dependency in C #

## Why bother?

A simple and lightweight event bus that can be used in any C# project without ceremony.
Can be used effectively in an Outbox pattern to process events in the same transaction synchronously
while keeping application parts loosely coupled.

## Read More

You can check out my article on
EventAggregator [HERE](https://sfmohassel.medium.com/event-aggregator-an-implementation-in-c-17fad5e6ed28)

# Installation

The package is available on nuget.org:

```powershell
PM> Install-Package libc.eventbus
```

# Simple Usage

Let's say we have a simple message event that has a text:

```csharp
public class SimpleMessage : IEvent {
    public string Text { get; private set; }

    public SimpleMessage(string text) {
        Text = text;
    }
}
```

Now we want that when a `SimpleMessage` is raised, print it in two distinct ways:

```csharp
public class PrintMessageRaw : IEventHandler<SimpleMessage> {
    public Task Handle(SimpleMessage ev) {
        // print message
        Console.WriteLine($"Raw: {ev.Text}");
        return Task.CompletedTask;
    }
}

public class PrintMessagePretty : IEventHandler<SimpleMessage> {
    public Task Handle(SimpleMessage ev) {
        // print message
        Console.WriteLine($"Pretty: {ev.Text}");
        return Task.CompletedTask;
    }
}
```

As you can see both `PrintMessageRaw` and `PrintMessagePretty` implement _
_`IEventHandler<SimpleMessage>`__.

## Put it together

_Read the comments please_

```csharp
public void Showcase_WithoutCatchAll() {
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
```

The console will show:

```text
Raw: a simple message
Pretty: a simple message
```

# Usage 2

There are times that we need to catch all the event (for example for logging purposes). There's a
`ICatchAllEventHandler` interface that you can
register in the bus. To test it, first add a new event:

```csharp
public class PrivateMessage : IEvent {
    public string Secret { get; private set; }

    public PrivateMessage(string secret) {
        Secret = secret;
    }
}
```

then an implementation of `ICatchAllEventHandler`:

```csharp
public class CatchAllMessages : ICatchAllEventHandler {
    public Task Handle(IEvent ev) {
        if (ev is SimpleMessage) {
            Console.WriteLine($"Caught SimpleMessage: {(ev as SimpleMessage).Text}");
        } else if (ev is PrivateMessage) {
            Console.WriteLine($"Caught PrivateMessage: {(ev as PrivateMessage).Secret}");
        }
        return Task.CompletedTask;
    }
}
```

## Put it together

_Read the comments please_

```csharp
public void Showcase_WithCatchAll() {
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
```

The console will show:

```text
Raw: a simple message
Pretty: a simple message
Caught SimpleMessage: a simple message
```

To see the full showcase __[click here](../libc.eventbus.tests/ShowCase.cs)__

### Contributions are welcome
