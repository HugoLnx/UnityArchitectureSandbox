namespace LnxArch
{
    public struct LnxValueSource<T>
    {
        public ILnxValueComponent<T> Component;
        public ILnxValueComponent<T> Channel;
    }
    public delegate void ChangeCallback<T>(T oldValue, T newValue, LnxValueSource<T> source);
    public delegate void WriteCallback<T>(T value, LnxValueSource<T> source);
    public interface ILnxValueComponent<T> : ILnxValueComponentLightweight<T>
    {
        T Read();
        void Write(T value, LnxValueSource<T> source = default, bool skipCallbacks = false);
        event ChangeCallback<T> OnChange;
        event WriteCallback<T> OnWrite;
    }
}