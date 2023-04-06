using System.Collections;
using System.Collections.Generic;
using ArchitectureSandbox.ZenjectConventions;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using Zenject;

namespace ArchitectureSandbox.Zen
{
    public class AcceleratedRotatable2D : MonoBehaviour, IRotatable2D
    {
        [SerializeField] private float _acceleration;
        [SerializeField] private float _deacceleration;
        [SerializeField] private float _maxSpeed;

        private Directionable2D _directionable;
        private SpeedAcceleration _acceleratedSpeed;
        private TickablePerState<RotationState> _tickable;

        public RotationState RotationState {
            get => _tickable.State;
            set => _tickable.StartTickingFor(value);
        }

        [Inject]
        public void Construct(
            [Inject(Id="FromAncestor")]
            Directionable2D directionable,
            SpeedAcceleration acceleratedSpeed = null,
            TickablePerState<RotationState> tickable = null)
        {
            _directionable = directionable;
            _acceleratedSpeed = acceleratedSpeed ?? new(
                acceleration: _acceleration,
                deacceleration: _deacceleration,
                maxSpeed: _maxSpeed
            );
            _tickable = tickable ?? new(state => state switch
            {
                RotationState.RotatingClockwise => TickAccelerateClockwise,
                RotationState.RotatingAntiClockwise => TickAccelerateAntiClockwise,
                RotationState.NoRotation => TickDeaccelerate,
                _ => null
            });
        }

        private void Update()
        {
            _tickable.Tick(Time.deltaTime);
        }

        private void TickAccelerateClockwise(float deltaTime)
        {
            _acceleratedSpeed.TickAccelerate(deltaTime, clockwise: true);
            ApplySpeed(deltaTime);
        }

        private void TickAccelerateAntiClockwise(float deltaTime)
        {
            _acceleratedSpeed.TickAccelerate(deltaTime, clockwise: false);
            ApplySpeed(deltaTime);
        }

        private void TickDeaccelerate(float deltaTime)
        {
            _acceleratedSpeed.TickDeaccelerate(deltaTime);
            ApplySpeed(deltaTime);
        }

        private void ApplySpeed(float deltaTime)
        {
            _directionable.RotateBy(_acceleratedSpeed.Speed * deltaTime);
        }
    }
}