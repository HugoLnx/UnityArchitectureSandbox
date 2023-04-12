using System;

namespace ArchitectureSandbox.LnxArchSandbox
{
    public class AcceleratedRotatable2D : IRotatable2DWithCallbacks
    {
        private readonly Direction2DComponent _direction;
        private readonly SpeedAcceleration _acceleratedSpeed;
        private RotationState _state;

        public AcceleratedRotatable2D(Direction2DComponent direction, SpeedAcceleration acceleratedSpeed)
        {
            _direction = direction;
            _acceleratedSpeed = acceleratedSpeed;
        }

        public static AcceleratedRotatable2D Create(
            Direction2DComponent direction,
            float acceleration,
            float deacceleration,
            float maxSpeed)
        {
            return new AcceleratedRotatable2D(
                direction,
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

        private void UpdateSpeed(float deltaTime)
        {
            switch (_state) {
            case RotationState.PushingAntiClockwise:
                _acceleratedSpeed.TickAccelerateForward(deltaTime);
                break;
            case RotationState.PushingClockwise:
                _acceleratedSpeed.TickAccelerateBackward(deltaTime);
                break;
            default:
                _acceleratedSpeed.TickDeaccelerate(deltaTime);
                break;
            };
        }

        private void ApplySpeed(float deltaTime)
        {
            _direction.Value = Math2D.Rotate(_direction.Value, byDegrees: _acceleratedSpeed.Speed * deltaTime);
        }
    }
}