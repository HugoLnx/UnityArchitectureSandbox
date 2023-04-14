using System;
using System.Collections;
using ArchitectureSandbox.ZenjectConventions;
using LnxArch;
using UnityEngine;
using UnityEngine.Assertions;

namespace ArchitectureSandbox.LnxArchSandbox
{
    public class PlayerLnxArchBehaviour : LnxBehaviour
    {
        private IRotatable2D _rotatable;
        private PlayerInput _input;
        private MonitoredValue<RotationState> _monitoredState;

        [Autofetch]
        private void Prepare(IRotatable2DBehaviour rotatableBehaviour)
        {
            _rotatable = rotatableBehaviour.Rotatable;
            _input = new PlayerInput();
            _monitoredState = new MonitoredValue<RotationState>(start: RotationState.NoPushing);
        }

        private void Update()
        {
            ProcessRotationInput(_input.AxisX);
        }

        private void ProcessRotationInput(float xInput)
        {
            RotationState state = xInput switch {
                > 0 => RotationState.PushingClockwise,
                < 0 => RotationState.PushingAntiClockwise,
                _ => RotationState.NoPushing,
            };
            _monitoredState.Update(state);
            if (_monitoredState.HasChanged) _rotatable.SwitchTo(_monitoredState.Value);
        }
    }
}