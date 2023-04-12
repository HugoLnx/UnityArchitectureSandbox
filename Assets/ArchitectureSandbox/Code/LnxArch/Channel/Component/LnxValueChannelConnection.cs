using UnityEngine;

namespace LnxArch
{
    public class LnxValueChannelConnection<T>
    {
        private readonly LnxValueChannel<T> _channel;
        private readonly LnxValueComponent<T> _component;
        private bool _isConnected;

        public LnxValueChannelConnection(LnxValueComponent<T> component, LnxValueChannel<T> channel)
        {
            _channel = channel;
            _component = component;
        }

        public void Connect()
        {
            if (_isConnected) return;
            _channel.OnWrite += OnChannelWrite;
            _component.OnWrite += OnComponentWrite;
            _isConnected = true;
        }

        public void Disconnect()
        {
            if (!_isConnected) return;
            _channel.OnWrite -= OnChannelWrite;
            _component.OnWrite -= OnComponentWrite;
            _isConnected = false;
        }

        private void OnChannelWrite(T value, LnxValueSource<T> source)
        {
            _component.Write(value, source);
        }

        private void OnComponentWrite(T value, LnxValueSource<T> source)
        {
            _channel.Write(value, source);
        }
    }
}