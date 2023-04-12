using UnityEngine;

namespace LnxArch
{
    public abstract class LnxEventChannelConnection<T> : LnxBehaviour
    {
        [SerializeField] private LnxEventChannel<T> _channel;
        [SerializeField] private LnxEvent<T> _event;
        [SerializeField] private bool _onlySyncWhenEnabled;
        private bool _linkIsSet;

        private void Awake()
        {
            if (_event == null) _event = GetComponent<LnxEvent<T>>();
            if (_event == null) _event = _entity.FetchFirst<LnxEvent<T>>();
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
            _channel.OnTrigger += OnChannelTrigger;
            _event.OnTrigger += OnEventTrigger;
            _linkIsSet = true;
        }

        private void Disconnect()
        {
            if (!_linkIsSet) return;
            _channel.OnTrigger -= OnChannelTrigger;
            _event.OnTrigger -= OnEventTrigger;
            _linkIsSet = false;
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