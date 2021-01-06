using libc.eventbus.System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using static libc.eventbus.tests.SimpleHandlerTests;

namespace libc.eventbus.tests {
    [TestClass]
    public class SubscriptionTests {
        [TestMethod]
        public void Subscribe_Unsubscribe() {
            var bus = new DefaultEventBus();
            var handler = new LogEventHandler1(null);
            bus.Subscribe<LogEvent, LogEventHandler1>(handler);
            var handlers = bus.GetHandlers<LogEvent>().ToArray();

            Assert.IsTrue(bus.GetCatchAllHandlers().Count() == 0, "No catch-all");
            Assert.IsTrue(handlers.Length == 1, "one handler");
            Assert.IsTrue(handlers[0].Equals(handler), "types ok");

            bus.Unsubscribe<LogEvent, LogEventHandler1>(handler);
            handlers = bus.GetHandlers<LogEvent>().ToArray();

            Assert.IsTrue(bus.GetCatchAllHandlers().Count() == 0, "No catch-all again");
            Assert.IsTrue(handlers.Length == 0, "no handler");
        }

        [TestMethod]
        public void Register_Unregister() {
            var bus = new DefaultEventBus();
            var handler = new CatchAllHandler(null, null);
            bus.RegisterCatchAllHandler(handler);
            var catchAllHandlers = bus.GetCatchAllHandlers().ToArray();

            Assert.IsTrue(catchAllHandlers.Length == 1, "One catch-all");
            Assert.IsTrue(catchAllHandlers[0].Equals(handler), "Types ok");

            bus.UnregisterCatchAllHandler(handler);
            catchAllHandlers = bus.GetCatchAllHandlers().ToArray();

            Assert.IsTrue(catchAllHandlers.Length == 0, "No catch-all");
        }
    }
}
