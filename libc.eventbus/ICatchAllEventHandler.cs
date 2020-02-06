namespace libc.eventbus {
    public interface ICatchAllEventHandler {
        void Handle(IEvent ev);
    }
}