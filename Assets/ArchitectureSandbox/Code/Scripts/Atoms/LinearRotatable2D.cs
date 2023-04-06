using System;
using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace ArchitectureSandbox.Atoms
{
    [RequireComponent(typeof(Directionable2D))]
    public class LinearRotatable2D : MonoBehaviour, IRotatable2D
    {
        [SerializeField] private float _speed;
        [SerializeField] private VoidBaseEventReference _rotationClockwiseStart;
        [SerializeField] private VoidBaseEventReference _rotationAntiClockwiseStart;
        [SerializeField] private VoidBaseEventReference _rotationStop;

        private Directionable2D _directionable;
        private Action<float> _clockwiseTick;
        private Action<float> _antiClockwiseTick;
        private Action<float> _tickAction;

        private void Awake()
        {
            _directionable = GetComponent<Directionable2D>();
            _clockwiseTick = (float deltaTime) => RotateTick(deltaTime, clockwise: true);
            _antiClockwiseTick = (float deltaTime) => RotateTick(deltaTime, clockwise: false);

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
            _tickAction = null;
        }

        private void RotateTick(float deltaTime, bool clockwise)
        {
            float angle = _speed * deltaTime * (clockwise ? -1f : 1f);
            _directionable.Direction = Quaternion.AngleAxis(angle, Vector3.forward) * _directionable.Direction;
        }
    }
}