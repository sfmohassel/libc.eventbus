namespace libc.eventbus {
    public interface IHub {
        void Publish<TEvent>(TEvent ev) where TEvent : IEvent;
    }
}