using UnityEngine;

namespace LnxArch
{
    public abstract class LnxValueChannelConnection<T> : LnxBehaviour
    {
        [SerializeField] private LnxValueChannel<T> _channel;
        [SerializeField] private LnxValueComponent<T> _component;
        [SerializeField] private bool _onlySyncWhenEnabled;
        private bool _linkIsSet;

        private void Awake()
        {
            if (_component == null) _component = GetComponent<LnxValueComponent<T>>();
            if (_component == null) _component = _entity.FetchFirst<LnxValueComponent<T>>();
            Connect();
        }

        private void OnEnable()
        {
            if (_onlySyncWhenEnabled)
            {
                Connect();
            }
        }

        private void OnDisable()
        {
            if (_onlySyncWhenEnabled)
            {
                Disconnect();
            }
        }

        private void Connect()
        {
            if (_linkIsSet) return;
            _channel.OnWrite += OnChannelWrite;
            _component.OnWrite += OnComponentWrite;
            _linkIsSet = true;
        }

        private void Disconnect()
        {
            if (!_linkIsSet) return;
            _channel.OnWrite -= OnChannelWrite;
            _component.OnWrite -= OnComponentWrite;
            _linkIsSet = false;
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