using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace ArchitectureSandbox.Zen2
{
    public class LinearRotatable2DBehaviour : MonoBehaviour, IRotatable2DBehaviour
    {
        [SerializeField] private float _speed;
        private IRotatable2DWithCallbacks _rotatable;
        public IRotatable2D Rotatable => _rotatable;

        [Inject]
        public void ConstructFromInjected(Directionable2DComponent directionable)
        {
            Construct(new LinearRotatable2D(directionable, _speed));
        }

        public void Construct(IRotatable2DWithCallbacks rotatable)
        {
            Assert.IsNotNull(rotatable);
            _rotatable = rotatable;
        }

        private void Update()
        {
            _rotatable.Tick(Time.deltaTime);
        }
    }
}