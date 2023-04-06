using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace ArchitectureSandbox.Atoms
{
    [RequireComponent(typeof(Directionable2D))]
    public class AcceleratedRotatable2D : MonoBehaviour, IRotatable2D
    {
        [SerializeField] private float _acceleration;
        [SerializeField] private float _deacceleration;
        [SerializeField] private float _maxSpeed;
        [SerializeField] private VoidBaseEventReference _rotationClockwiseStart;
        [SerializeField] private VoidBaseEventReference _rotationAntiClockwiseStart;
        [SerializeField] private VoidBaseEventReference _rotationStop;

        private float _speed;
        private Directionable2D _directionable;
        private Action<float> _clockwiseTick;
        private Action<float> _antiClockwiseTick;
        private Action<float> _deaccelerateTick;
        private Action<float> _tickAction;

        private void Awake()
        {
            _directionable = GetComponent<Directionable2D>();
            _clockwiseTick = (float deltaTime) => RotateTick(deltaTime, clockwise: true);
            _antiClockwiseTick = (float deltaTime) => RotateTick(deltaTime, clockwise: false);
            _deaccelerateTick = (float deltaTime) => DeaccelerateTick(deltaTime);

            _rotationClockwiseStart?.Event?.Register(ClockwiseRotationStart);
            _rotationAntiClockwiseStart?.Event?.Register(AntiClockwiseRotationStart);
            _rotationStop?.Event?.Register(RotationStop);
        }

        private void Update()
        {
            _tickAction?.Invoke(Time.deltaTime);
        }

        public void ClockwiseRotationStart()
        {
            _tickAction = _clockwiseTick;
        }

        public void AntiClockwiseRotationStart()
        {
            _tickAction = _antiClockwiseTick;
        }

        public void RotationStop()
        {
            _tickAction = _deaccelerateTick;
        }

        private void RotateTick(float deltaTime, bool clockwise)
        {
            ApplySpeed(deltaTime);
            AccelerateSpeed(deltaTime, clockwise);
        }

        private void DeaccelerateTick(float deltaTime)
        {
            ApplySpeed(deltaTime);
            DeaccelerateSpeed(deltaTime);
        }

        private void ApplySpeed(float deltaTime)
        {
            float angle = _speed * deltaTime;
            _directionable.Direction = Quaternion.AngleAxis(angle, Vector3.forward) * _directionable.Direction;
        }

        private void AccelerateSpeed(float deltaTime, bool clockwise)
        {
            float accelerationSign = clockwise ? -1f : 1f;
            bool isAcceleratingInOpositeDirection =
                _speed != 0 && accelerationSign != Mathf.Sign(_speed);
            if (isAcceleratingInOpositeDirection)
            {
                deltaTime = DeaccelerateSpeed(deltaTime, increment: _acceleration);
            }
            _speed += _acceleration * accelerationSign * deltaTime;
            _speed = Mathf.Clamp(_speed, -_maxSpeed, _maxSpeed);
        }

        private float DeaccelerateSpeed(float deltaTime, float increment=0f)
        {
            float deacceleration = _deacceleration + increment;
            float step = deacceleration * deltaTime;
            float speedSign = Mathf.Sign(_speed);
            float unusedTime = 0;
            if (step >= _speed*speedSign)
            {
                _speed = 0;
                unusedTime = (step - (_speed * speedSign)) / deacceleration;
            }
            else
            {
                _speed -= speedSign * step;
            }
            return unusedTime;
        }
    }
}