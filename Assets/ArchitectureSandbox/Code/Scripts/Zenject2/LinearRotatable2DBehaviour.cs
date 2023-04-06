using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ArchitectureSandbox.Zen2
{
    public class LinearRotatable2DBehaviour : MonoBehaviour, IRotatable2DBehaviour
    {
        [SerializeField] private float _speed;
        private IRotatable2DInternal _rotatable;
        public IRotatable2D Rotatable => _rotatable;

        [Inject]
        public void Construct(
            [Inject(Id = "FromAncestor")]
            Directionable2D directionable,
            IRotatable2DInternal rotatable = null)
        {
            _rotatable = rotatable ?? new LinearRotatable2D(directionable, _speed);
        }

        private void Update()
        {
            _rotatable.Tick(Time.deltaTime);
        }
    }
}