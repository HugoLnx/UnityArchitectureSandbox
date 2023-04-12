using UnityEngine;

namespace LnxArch
{
    public abstract class LnxValueComponentSynchronized<T> : LnxValueComponent<T>
    {
        [SerializeField] private bool _syncOnUpdate;
        [SerializeField] private bool _syncOnFixedUpdate;
        private T _lastChangeNotified;

        protected void Awake()
        {
            OnChange += (_, _, newValue) => _lastChangeNotified = newValue;
        }

        protected void Update()
        {
            if (!_syncOnUpdate) return;
            if (!IsEquals(_lastChangeNotified, Value))
            {
                EmitChange(_lastChangeNotified, Value);
            }
        }

        protected void FixedUpdate()
        {
            if (!_syncOnFixedUpdate) return;
            if (!IsEquals(_lastChangeNotified, Value))
            {
                EmitChange(_lastChangeNotified, Value);
            }
        }
    }
}