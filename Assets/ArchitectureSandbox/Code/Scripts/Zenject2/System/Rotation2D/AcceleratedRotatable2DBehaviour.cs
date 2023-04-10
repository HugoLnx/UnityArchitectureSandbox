using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace ArchitectureSandbox.Zen2
{
    public class AcceleratedRotatable2DBehaviour : MonoBehaviour, IRotatable2DBehaviour
    {
        [SerializeField] private float _acceleration;
        [SerializeField] private float _deacceleration;
        [SerializeField] private float _maxSpeed;
        private IRotatable2DWithCallbacks _rotatable;
        public IRotatable2D Rotatable => _rotatable;

        [Inject]
        public void ConstructFromInjected(Directionable2DComponent directionable)
        {
            Debug.Log($"AcceleratedConstructFromInjected {directionable}");
            Construct(AcceleratedRotatable2D.Create(directionable, _acceleration, _deacceleration, _maxSpeed));
        }

        public void Construct(IRotatable2DWithCallbacks rotatable)
        {
            Debug.Log($"AcceleratedConstruct {rotatable}");
            Assert.IsNotNull(rotatable);
            _rotatable = rotatable;
        }

        private void Update()
        {
            _rotatable.Tick(Time.deltaTime);
        }
    }
}