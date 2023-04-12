namespace LnxArch
{
    public struct LnxComponentCallbackSource<T>
    {
        public ILnxValueComponent<T> Component;
        public ILnxValueComponent<T> Channel;
    }
    public delegate void ChangeCallback<T>(T oldValue, T newValue, LnxComponentCallbackSource<T> source);
    public delegate void WriteCallback<T>(T value, LnxComponentCallbackSource<T> source);
    public interface ILnxValueComponent<T> : ILnxValueComponentLightweight<T>
    {
        T Read();
        void Write(T value, LnxComponentCallbackSource<T> source = default, bool skipCallbacks = false);
        event ChangeCallback<T> OnChange;
        event WriteCallback<T> OnWrite;
    }
}