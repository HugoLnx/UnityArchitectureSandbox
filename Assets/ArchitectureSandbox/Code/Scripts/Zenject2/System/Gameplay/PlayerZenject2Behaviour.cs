using System;
using System.Collections;
using ArchitectureSandbox.ZenjectConventions;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace ArchitectureSandbox.Zen2
{
    public class PlayerZenject2Behaviour : MonoBehaviour
    {
        private IRotatable2D _rotatable;
        private PlayerInput _input;
        private MonitoredValue<RotationState> _monitoredState;

        public void Construct(IRotatable2D rotatable, PlayerInput input, MonitoredValue<RotationState> monitoredState)
        {
            Assert.IsNotNull(_rotatable);
            Assert.IsNotNull(_input);
            Assert.IsNotNull(_monitoredState);

            _rotatable = rotatable;
            _input = input;
            _monitoredState = monitoredState;
        }

        [Inject]
        public void ConstructFromInjected(IRotatable2DBehaviour rotatableBehaviour)
        {
            Assert.IsNotNull(rotatableBehaviour);
            Construct(
                rotatable: rotatableBehaviour.Rotatable,
                input: new PlayerInput(),
                monitoredState: new MonitoredValue<RotationState>(start: RotationState.NoPushing)
            );
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