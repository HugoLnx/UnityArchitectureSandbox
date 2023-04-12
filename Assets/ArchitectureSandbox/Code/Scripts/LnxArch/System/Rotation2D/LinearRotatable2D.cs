using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchitectureSandbox.LnxArchSandbox
{
    public class LinearRotatable2D : IRotatable2DWithCallbacks
    {
        private readonly Direction2DComponent _direction;
        private readonly float _speed;
        private RotationState _state;
        private float OrientationModifier => _state switch
        {
            RotationState.PushingAntiClockwise => 1f,
            RotationState.PushingClockwise => -1f,
            _ => 0,
        };

        public LinearRotatable2D(Direction2DComponent direction, float speed)
        {
            _direction = direction;
            _speed = speed;
        }

        public void Tick(float deltaTime)
        {
            if (_state == RotationState.NoPushing) return;
            _direction.Value = Math2D.Rotate(_direction.Value, byDegrees: _speed * deltaTime * OrientationModifier);
        }

        public void SwitchTo(RotationState state)
        {
            _state = state;
        }
    }
}