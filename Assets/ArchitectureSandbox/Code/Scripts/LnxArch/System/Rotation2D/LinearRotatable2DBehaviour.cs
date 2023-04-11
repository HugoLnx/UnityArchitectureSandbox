using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using LnxArch;

namespace ArchitectureSandbox.LnxArchSandbox
{
    public class LinearRotatable2DBehaviour : LnxBehaviour, IRotatable2DBehaviour
    {
        [SerializeField] private float _speed;
        private IRotatable2DWithCallbacks _rotatable;

        public IRotatable2D Rotatable => _rotatable;

        [AutoFetch]
        private void Prepare(Directionable2DComponent directionable)
        {
            _rotatable = new LinearRotatable2D(directionable, _speed);
        }

        private void Update()
        {
            _rotatable.Tick(Time.deltaTime);
        }
    }
}