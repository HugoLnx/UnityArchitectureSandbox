using UnityEngine;

namespace LnxArch
{
    public abstract class LnxComponentSynchronizer<T, K> : LnxBehaviour
    where K : LnxComponent<T>
    {
        [SerializeField] private K _component;
        [SerializeField] private bool _writeOnUpdate = true;
        [SerializeField] private bool _writeOnFixedUpdate = false;

        protected abstract void Push(T value);
        protected abstract T Pull();

        private void Awake()
        {
            if (_component == null) _component = GetComponent<K>();
            if (_component == null) _component = Entity.FetchFirst<K>();
            _component.OnWrite += (value, _) => Push(value);
            _component.OverwriteReadingTo(Pull);
        }

        protected void Update()
        {
            if (!_writeOnUpdate) return;
            _component.Write(Pull());
        }

        protected void FixedUpdate()
        {
            if (!_writeOnFixedUpdate) return;
            _component.Write(Pull());
        }
    }
}