namespace LnxArch
{
    public struct LnxComponentSource<T>
    {
        public ILnxComponent<T> Component;
        public ILnxComponent<T> Channel;
    }
    public delegate void ChangeCallback<T>(T oldValue, T newValue, LnxComponentSource<T> source);
    public delegate void WriteCallback<T>(T value, LnxComponentSource<T> source);
    public interface ILnxComponent<T> : ILnxComponentLightweight<T>
    {
        T Read();
        void Write(T value, LnxComponentSource<T> source = default, bool skipCallbacks = false);
        event ChangeCallback<T> OnChange;
        event WriteCallback<T> OnWrite;
    }
}