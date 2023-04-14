using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LnxArch
{
    public abstract class LnxEventBase<T> : MonoBehaviour, ILnxEvent<T>
    {
        public event EventCallback<T> OnTrigger;

        public void Emit(T args = default, LnxEventTriggerSource<T> source = default)
        {
            if ((object) source.Event == this) return;
            source.Event = this;
            OnTrigger?.Invoke(args, source);
        }
    }

    public abstract class LnxEvent<T> : LnxEventBase<T>
    {
        [SerializeField] private LnxEventChannel<T> _channel;
        [SerializeField] private bool _disconnectChannelOnDisable;
        private LnxEventChannelConnection<T> _channelConnection;
        private void Awake()
        {
            if (_channel != null)
            {
                _channelConnection = new LnxEventChannelConnection<T>(this, _channel);
                _channelConnection.Connect();
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