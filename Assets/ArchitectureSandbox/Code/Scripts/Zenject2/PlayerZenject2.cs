using System.Collections;
using System.Collections.Generic;
using ArchitectureSandbox.ZenjectConventions;
using UnityEngine;
using Zenject;

namespace ArchitectureSandbox.Zen2
{
    public class PlayerZenject2 : MonoBehaviour
    {
        private const string HorizontalAxisName = "Horizontal";
        private IRotatable2D _rotatable;

        [Inject]
        public void Construct(IRotatable2DBehaviour rotatableBehaviour)
        {
            _rotatable = rotatableBehaviour.Rotatable;
        }

        private void Update()
        {
            ProcessRotate(Input.GetAxis(HorizontalAxisName));
        }

        private void ProcessRotate(float xInput)
        {
            RotationState state = xInput switch {
                > 0 => RotationState.PushingClockwise,
                < 0 => RotationState.PushingAntiClockwise,
                _ => RotationState.NoPushing,
            };
            _rotatable.SwitchTo(state);
        }
    }
}