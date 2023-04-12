using System.Collections.Generic;

namespace LnxArch
{
    public abstract class LnxValueComponent<T> : LnxValueComponentLightweight<T>, ILnxValueComponent<T>
    {
        public delegate void ChangeCallback(ILnxValueComponentLightweight<T> source, T oldValue, T newValue);
        public delegate void SetCallback(ILnxValueComponentLightweight<T> source, T value);
        public event ChangeCallback OnChange;
        public event SetCallback OnSet;
        protected virtual T PlainValue {
            get => _value;
            set => _value = value;
        }

        public override T Value {
            get => Pull();
            set => Push(value);
        }

        public T Pull()
        {
            return PlainValue;
        }

        public void Push(T value, bool skipCallbacks = false)
        {
            T oldValue = PlainValue;
            PlainValue = value;

            if (skipCallbacks) return;
            OnSet?.Invoke(this, PlainValue);
            if (!IsEquals(oldValue, PlainValue))
            {
                EmitChange(oldValue, PlainValue);
            }
        }

        protected void EmitChange(T oldValue, T newValue)
        {
            OnChange?.Invoke(this, oldValue, newValue);
        }

        public static bool IsEquals<K>(K v1, K v2)
        {
            return EqualityComparer<K>.Default.Equals(v1, v2);
        }
    }
}