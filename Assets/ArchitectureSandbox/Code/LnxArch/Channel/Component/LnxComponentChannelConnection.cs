using UnityEngine;

namespace LnxArch
{
    public class LnxComponentChannelConnection<T>
    {
        private readonly LnxComponentChannel<T> _channel;
        private readonly LnxComponent<T> _component;
        private bool _isConnected;

        public LnxComponentChannelConnection(LnxComponent<T> component, LnxComponentChannel<T> channel)
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

        private void OnChannelWrite(T value, LnxComponentSource<T> source)
        {
            _component.Write(value, source);
        }

        private void OnComponentWrite(T value, LnxComponentSource<T> source)
        {
            _channel.Write(value, source);
        }
    }
}