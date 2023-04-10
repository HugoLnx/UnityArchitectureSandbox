using System;

namespace ArchitectureSandbox.Zen2
{
    public class AcceleratedRotatable2D : IRotatable2DWithCallbacks
    {
        private readonly Directionable2DComponent _directionable;
        private readonly SpeedAcceleration _acceleratedSpeed;
        private RotationState _state;

        public AcceleratedRotatable2D(Directionable2DComponent directionable, SpeedAcceleration acceleratedSpeed)
        {
            _directionable = directionable;
            _acceleratedSpeed = acceleratedSpeed;
        }

        public static AcceleratedRotatable2D Create(
            Directionable2DComponent directionable,
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
            _directionable.RotateBy(_acceleratedSpeed.Speed * deltaTime);
        }
    }
}