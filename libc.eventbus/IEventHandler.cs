namespace libc.eventbus {
    public interface IEventHandler {
    }
    public interface IEventHandler<in TEvent> : IEventHandler where TEvent : IEvent {
        void Handle(TEvent ev);
    }
}