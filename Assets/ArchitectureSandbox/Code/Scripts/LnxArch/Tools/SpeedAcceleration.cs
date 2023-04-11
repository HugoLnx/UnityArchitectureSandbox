namespace ArchitectureSandbox.LnxArchSandbox
{
    public class SpeedAcceleration
    {
        public float Speed {get; private set;}
        private float _acceleration;
        private float _deacceleration;
        private float _maxSpeed;

        public SpeedAcceleration(float acceleration, float deacceleration, float maxSpeed)
        {
            _acceleration = acceleration;
            _deacceleration = deacceleration;
            _maxSpeed = maxSpeed;
        }

        public void TickAccelerateForward(float deltaTime)
        {
            AccelerateSpeed(deltaTime, forward: true);
        }

        public void TickAccelerateBackward(float deltaTime)
        {
            AccelerateSpeed(deltaTime, forward: false);
        }

        public void TickDeaccelerate(float deltaTime)
        {
            DeaccelerateSpeed(deltaTime);
        }

        private void AccelerateSpeed(float deltaTime, bool forward)
        {
            float accelerationSign = forward ? 1f : -1f;
            bool isAcceleratingInOpositeDirection =
                Speed != 0 && accelerationSign != Sign(Speed);
            if (isAcceleratingInOpositeDirection)
            {
                deltaTime = DeaccelerateSpeed(deltaTime, increment: _acceleration);
            }
            Speed += _acceleration * accelerationSign * deltaTime;
            Speed = Clamp(Speed, -_maxSpeed, _maxSpeed);
        }

        private float DeaccelerateSpeed(float deltaTime, float increment=0f)
        {
            float deacceleration = _deacceleration + increment;
            float step = deacceleration * deltaTime;
            float speedSign = Sign(Speed);
            float unusedTime = 0;
            if (step >= Speed*speedSign)
            {
                Speed = 0;
                unusedTime = (step - (Speed * speedSign)) / deacceleration;
            }
            else
            {
                Speed -= speedSign * step;
            }
            return unusedTime;
        }

        private float Clamp(float val, float min, float max)
        {
            if (val <= min) return min;
            if (val >= max) return max;
            return val;
        }

        private float Sign(float val)
        {
            if (val > 0) return 1f;
            if (val < 0) return -1f;
            return 0;
        }
    }
}