using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LnxArch
{
    public abstract class LnxEventComponentBase : MonoBehaviour
    {
    }

    public abstract class LnxEventComponent<T> : LnxEventComponentBase
    where T : LnxEventComponent<T>
    {
        public delegate void Callback(T source);
        public event Callback OnTrigger;

        public void Emit()
        {
            OnTrigger?.Invoke((T) this);
        }
    }

    public abstract class LnxEventComponent<T, K> : LnxEventComponentBase
    where T : LnxEventComponent<T, K>
    {
        public delegate void Callback(T source, K args);
        public event Callback OnTrigger;

        public void Emit(K args)
        {
            OnTrigger?.Invoke((T) this, args);
        }
    }
}