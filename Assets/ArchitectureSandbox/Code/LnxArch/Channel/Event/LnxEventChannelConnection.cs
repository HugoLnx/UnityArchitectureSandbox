using UnityEngine;

namespace LnxArch
{
    public enum ChannelConnectionMode {
        Disconnected, AlwaysSync, SyncWhenEnabled
    }

    public class LnxEventChannelConnection<T>
    {
        private LnxEvent<T> _event;
        private LnxEventChannel<T> _channel;
        private bool _isConnected;

        public LnxEventChannelConnection(LnxEvent<T> evt, LnxEventChannel<T> channel)
        {
            _event = evt;
            _channel = channel;
        }

        public void Connect()
        {
            if (_isConnected) return;
            _channel.OnTrigger += OnChannelTrigger;
            _event.OnTrigger += OnEventTrigger;
            _isConnected = true;
        }

        public void Disconnect()
        {
            if (!_isConnected) return;
            _channel.OnTrigger -= OnChannelTrigger;
            _event.OnTrigger -= OnEventTrigger;
            _isConnected = false;
        }

        private void OnChannelTrigger(T args, LnxEventTriggerSource<T> source)
        {
            _event.Emit(args, source);
        }

        private void OnEventTrigger(T args, LnxEventTriggerSource<T> source)
        {
            _channel.Emit(args, source);
        }
    }
}