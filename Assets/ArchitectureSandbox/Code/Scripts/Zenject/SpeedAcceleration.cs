using System;
using UnityEngine;

namespace ArchitectureSandbox.Zen
{
    public class SpeedAcceleration
    {
        public float Speed {get; private set;}
        private float _acceleration;
        private float _deacceleration;
        private float _maxSpeed;
        private readonly Action<float> _onTickApplySpeed;

        public SpeedAcceleration(float acceleration, float deacceleration, float maxSpeed, Action<float> onTickApplySpeed = null)
        {
            _acceleration = acceleration;
            _deacceleration = deacceleration;
            _maxSpeed = maxSpeed;
            _onTickApplySpeed = onTickApplySpeed;
        }

        public void TickAccelerate(float deltaTime, bool clockwise)
        {
            ApplySpeed(deltaTime);
            AccelerateSpeed(deltaTime, clockwise);
        }

        public void TickDeaccelerate(float deltaTime)
        {
            ApplySpeed(deltaTime);
            DeaccelerateSpeed(deltaTime);
        }

        private void ApplySpeed(float deltaTime)
        {
            _onTickApplySpeed?.Invoke(Speed * deltaTime);
        }

        private void AccelerateSpeed(float deltaTime, bool clockwise)
        {
            float accelerationSign = clockwise ? -1f : 1f;
            bool isAcceleratingInOpositeDirection =
                Speed != 0 && accelerationSign != Mathf.Sign(Speed);
            if (isAcceleratingInOpositeDirection)
            {
                deltaTime = DeaccelerateSpeed(deltaTime, increment: _acceleration);
            }
            Speed += _acceleration * accelerationSign * deltaTime;
            Speed = Mathf.Clamp(Speed, -_maxSpeed, _maxSpeed);
        }

        private float DeaccelerateSpeed(float deltaTime, float increment=0f)
        {
            float deacceleration = _deacceleration + increment;
            float step = deacceleration * deltaTime;
            float speedSign = Mathf.Sign(Speed);
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
    }
}