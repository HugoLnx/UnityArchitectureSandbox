namespace LnxArch
{
    public interface ILnxValueComponentLightweight<T>
    {
        T Value { get; set; }
    }

    public interface ILnxValueComponent<T> : ILnxValueComponentLightweight<T>
    {
        T Pull();
        void Push(T value, bool skipCallbacks = false);
    }
}