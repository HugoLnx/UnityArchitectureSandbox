using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LnxArch
{
    public abstract class LnxEvent<T> : MonoBehaviour, ILnxEvent<T>
    {
        public event EventCallback<T> OnTrigger;

        public void Emit(T args = default, LnxEventTriggerSource<T> source = default)
        {
            if (source.Event == this) return;
            source.Event = this;
            OnTrigger?.Invoke(args, source);
        }
    }
}