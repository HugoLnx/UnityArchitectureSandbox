using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchitectureSandbox.Zen2
{
    public class LinearRotatable2D : IRotatable2DInternal
    {
        private readonly Directionable2D _directionable;
        private readonly float _speed;
        private RotationState _state;
        private float OrientationModifier => _state switch
        {
            RotationState.PushingAntiClockwise => 1f,
            RotationState.PushingClockwise => -1f,
            _ => 0,
        };

        public LinearRotatable2D(Directionable2D directionable, float speed)
        {
            _directionable = directionable;
            _speed = speed;
        }

        public void Tick(float deltaTime)
        {
            if (_state == RotationState.NoPushing) return;
            _directionable.RotateBy(_speed * deltaTime * OrientationModifier);
        }

        public void SwitchTo(RotationState state)
        {
            _state = state;
        }
    }
}