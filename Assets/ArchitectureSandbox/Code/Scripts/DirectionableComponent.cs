using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchitectureSandbox
{
    public class DirectionableComponent : MonoBehaviour
    {
        private enum RotatingState {None, Clockwise, AntiClockwise}
        [SerializeField] private float _speed;
        private RotatingState _state;

        public Vector2 Direction {
            get => toXZ(this.transform.forward);
            set => this.transform.forward = toX0Y(value);
        }

        private void Update()
        {
            switch (_state)
            {
            case RotatingState.Clockwise:
                ApplyRotation(-Time.deltaTime);
                break;
            case RotatingState.AntiClockwise:
                ApplyRotation(Time.deltaTime);
                break;
            };
        }

        private void ApplyRotation(float deltaTime)
        {
            float angle = _speed * deltaTime;
            Direction = Quaternion.AngleAxis(angle, Vector3.forward) * Direction;
        }

        public void ClockwiseRotationStart()
        {
            _state = RotatingState.Clockwise;
        }

        public void AntiClockwiseRotationStart()
        {
            _state = RotatingState.AntiClockwise;
        }

        public void RotationStop()
        {
            _state = RotatingState.None;
        }

        private Vector3 toX0Y(Vector2 v) => new(v.x, 0f, v.y);
        private Vector3 toX0Y(Vector3 v) => toX0Y(v);
        private Vector2 toXZ(Vector3 v) => new(v.x, v.z);
    }
}