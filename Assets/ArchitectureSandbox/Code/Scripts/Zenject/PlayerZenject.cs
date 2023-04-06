using System.Collections;
using System.Collections.Generic;
using ArchitectureSandbox.ZenjectConventions;
using UnityEngine;
using Zenject;

namespace ArchitectureSandbox.Zen
{
    public class PlayerZenject : MonoBehaviour
    {
        private const string HorizontalAxisName = "Horizontal";
        private IRotatable2D _rotatable;

        [Inject]
        public void Construct(IRotatable2D rotatable)
        {
            _rotatable = rotatable;
        }

        private void Update()
        {
            ProcessRotate(Input.GetAxis(HorizontalAxisName));
        }

        private void ProcessRotate(float xInput)
        {
            if (xInput == 0) _rotatable.RotationState = RotationState.NoRotation;
            else if (xInput > 0) _rotatable.RotationState = RotationState.RotatingClockwise;
            else _rotatable.RotationState = RotationState.RotatingAntiClockwise;
        }
    }
}