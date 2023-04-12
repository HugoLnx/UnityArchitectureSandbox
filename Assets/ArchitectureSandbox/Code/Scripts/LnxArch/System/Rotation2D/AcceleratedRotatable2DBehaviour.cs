using LnxArch;
using UnityEngine;
using UnityEngine.Assertions;

namespace ArchitectureSandbox.LnxArchSandbox
{
    public class AcceleratedRotatable2DBehaviour : LnxBehaviour, IRotatable2DBehaviour
    {
        [SerializeField] private float _acceleration;
        [SerializeField] private float _deacceleration;
        [SerializeField] private float _maxSpeed;
        private IRotatable2DWithCallbacks _rotatable;
        public IRotatable2D Rotatable => _rotatable;

        [AutoFetch]
        public void Prepare(Direction2DComponent direction)
        {
            _rotatable = AcceleratedRotatable2D.Create(direction, _acceleration, _deacceleration, _maxSpeed);
        }

        private void Update()
        {
            _rotatable.Tick(Time.deltaTime);
        }
    }
}