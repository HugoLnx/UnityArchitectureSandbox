using UnityEngine;
using Zenject;

namespace ArchitectureSandbox.Zen2
{
    public class AcceleratedRotatable2DBehaviour : MonoBehaviour, IRotatable2DBehaviour
    {
        [SerializeField] private float _acceleration;
        [SerializeField] private float _deacceleration;
        [SerializeField] private float _maxSpeed;
        private IRotatable2DInternal _rotatable;
        public IRotatable2D Rotatable => _rotatable;

        [Inject]
        public void Construct(
            [Inject(Id = "FromAncestor")]
            Directionable2D directionable,
            IRotatable2DInternal rotatable = null)
        {
            _rotatable = rotatable ?? AcceleratedRotatable2D.Build(directionable, _acceleration, _deacceleration, _maxSpeed);
        }

        private void Update()
        {
            _rotatable.Tick(Time.deltaTime);
        }
    }
}