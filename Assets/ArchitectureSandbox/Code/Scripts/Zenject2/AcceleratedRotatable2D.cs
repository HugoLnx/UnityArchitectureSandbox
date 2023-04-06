using System;

namespace ArchitectureSandbox.Zen2
{
    public class AcceleratedRotatable2D : IRotatable2DInternal
    {
        private readonly Directionable2D _directionable;
        private readonly SpeedAcceleration _acceleratedSpeed;
        private RotationState _state;
        private readonly Action<float> _actionAccelerateForward;
        private readonly Action<float> _actionAccelerateBackward;
        private readonly Action<float> _actionDeaccelerate;
        private Action<float> UpdateSpeed => _state switch {
            RotationState.PushingAntiClockwise => _actionAccelerateForward,
            RotationState.PushingClockwise => _actionAccelerateBackward,
            _ => _actionDeaccelerate
        };

        public AcceleratedRotatable2D(
            Directionable2D directionable,
            SpeedAcceleration acceleratedSpeed)
        {
            _directionable = directionable;
            _acceleratedSpeed = acceleratedSpeed;
            _actionAccelerateForward = acceleratedSpeed.TickAccelerateForward;
            _actionAccelerateBackward = acceleratedSpeed.TickAccelerateBackward;
            _actionDeaccelerate = acceleratedSpeed.TickDeaccelerate;
        }

        public static AcceleratedRotatable2D Build(
            Directionable2D directionable,
            float acceleration,
            float deacceleration,
            float maxSpeed)
        {
            return new AcceleratedRotatable2D(
                directionable,
                acceleratedSpeed: new(acceleration, deacceleration, maxSpeed));
        }

        public void SwitchTo(RotationState state)
        {
            _state = state;
        }

        public void Tick(float deltaTime)
        {
            UpdateSpeed(deltaTime);
            ApplySpeed(deltaTime);
        }

        private void ApplySpeed(float deltaTime)
        {
            _directionable.RotateBy(_acceleratedSpeed.Speed * deltaTime);
        }
    }
}