using UnityEngine;


namespace LnxArch
{
    public abstract class LnxEventChannel<T> : ScriptableObject, ILnxEvent<T>
    {
        public event EventCallback<T> OnTrigger;

        public void Emit(T args = default, LnxEventTriggerSource<T> source = default)
        {
            if (source.Channel == this) return;
            source.Channel = this;
            OnTrigger?.Invoke(args, source);
        }
    }
}