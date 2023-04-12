using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(menuName = "LnxArch/LnxValueChannel", fileName = "new #SCRIPTNAME#")]
namespace LnxArch
{
    public abstract class LnxValueChannel<T> : ScriptableObject, ILnxValueComponent<T>
    {
        public event ChangeCallback<T> OnChange;
        public event WriteCallback<T> OnWrite;
        private T _value;
        public T Value {
            get => Read();
            set => Write(value);
        }
        public T Read()
        {
            return _value;
        }

        public void Write(T value, LnxComponentCallbackSource<T> source = default, bool skipCallbacks = false)
        {
            if (source.Channel == this) return;
            source.Channel = this;
            T oldValue = _value;
            _value = value;
            OnWrite?.Invoke(_value, source);
            if (!IsEquals(oldValue, _value))
            {
                OnChange?.Invoke(oldValue, _value, source);
            }
        }

        private static bool IsEquals<K>(K v1, K v2)
        {
            return EqualityComparer<K>.Default.Equals(v1, v2);
        }
    }
}