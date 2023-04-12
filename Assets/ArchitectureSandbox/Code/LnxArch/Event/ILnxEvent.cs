namespace LnxArch
{
    public struct LnxEventTriggerSource<T>
    {
        public ILnxEvent<T> Event;
        public LnxEventChannel<T> Channel;
    }

    public delegate void EventCallback<T>(T args = default, LnxEventTriggerSource<T> source = default);

    public interface ILnxEvent<T>
    {
        event EventCallback<T> OnTrigger;
        void Emit(T args = default, LnxEventTriggerSource<T> source = default);
    }
}