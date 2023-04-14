using System;
using System.Collections.Generic;
using UnityEngine;

namespace LnxArch
{
    public abstract class LnxComponentBase<T> : LnxComponentLightweight<T>, ILnxComponent<T>
    {
        public event ChangeCallback<T> OnChange;
        public event WriteCallback<T> OnWrite;
        private System.Func<T> ReadOverwrite;

        protected virtual T PlainValue {
            get => _value;
            set => _value = value;
        }

        public override T Value {
            get => Read();
            set => Write(value);
        }

        public T Read()
        {
            return ReadOverwrite == null ? PlainValue : ReadOverwrite();
        }

        public void Write(T value, LnxComponentSource<T> source = default, bool skipCallbacks = false)
        {
            if ((object) source.Component == this) return;
            T oldValue = PlainValue;
            PlainValue = value;

            if (skipCallbacks) return;
            EmitWrite(value, source);
            if (!IsEquals(oldValue, PlainValue))
            {
                EmitChange(oldValue, PlainValue, source);
            }
        }

        public void OverwriteReadingTo(Func<T> read)
        {
            if (ReadOverwrite != null)
            {
                throw new InvalidReadingOverwrite($"Reading was already overwritten on Component {this.GetType().Name}.");
            }
            ReadOverwrite = read;
        }

        protected void EmitChange(T oldValue, T newValue, LnxComponentSource<T> source = default)
        {
            source.Component = this;
            OnChange?.Invoke(oldValue, newValue, source);
        }

        protected void EmitWrite(T value, LnxComponentSource<T> source = default)
        {
            source.Component = this;
            OnWrite?.Invoke(value, source);
        }

        private static bool IsEquals<K>(K v1, K v2)
        {
            return EqualityComparer<K>.Default.Equals(v1, v2);
        }
    }

    public abstract class LnxComponent<T> : LnxComponentBase<T>
    {
        [SerializeField] private LnxComponentChannel<T> _channel;
        [SerializeField] private bool _disconnectChannelOnDisable;
        private LnxComponentChannelConnection<T> _channelConnection;

        private void Awake()
        {
            if (_channel != null)
            {
                _channelConnection = new LnxComponentChannelConnection<T>(this, _channel);
            }
        }

        private void OnEnable()
        {
            _channelConnection?.Connect();
        }

        private void OnDisable()
        {
            if (_disconnectChannelOnDisable)
            {
                _channelConnection?.Disconnect();
            }
        }
    }
}