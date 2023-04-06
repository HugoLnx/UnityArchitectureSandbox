using System;
using System.Collections;
using System.Collections.Generic;
using ArchitectureSandbox.ZenjectConventions;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using Zenject;

namespace ArchitectureSandbox.Zen
{
    public class LinearRotatable2D : MonoBehaviour, IRotatable2D
    {
        [SerializeField] private float _speed;

        private Directionable2D _directionable;
        private TickablePerState<RotationState> _tickable;

        public RotationState RotationState {
            get => _tickable.State;
            set => _tickable.StartTickingFor(value);
        }

        [Inject]
        public void Construct(
            [Inject(Id="FromAncestor")]
            Directionable2D directionable,
            TickablePerState<RotationState> tickable = null)
        {
            _directionable = directionable;
            _tickable = tickable ?? new(state => state switch
            {
                RotationState.RotatingClockwise => deltaTime => RotateTick(deltaTime, clockwise: true),
                RotationState.RotatingAntiClockwise =>  deltaTime => RotateTick(deltaTime, clockwise: false),
                RotationState.NoRotation => null,
                _ => null
            });
        }

        private void Update()
        {
            _tickable.Tick(Time.deltaTime);
        }

        private void RotateTick(float deltaTime, bool clockwise)
        {
            float angle = _speed * deltaTime * (clockwise ? -1f : 1f);
            _directionable.Direction = Quaternion.AngleAxis(angle, Vector3.forward) * _directionable.Direction;
        }
    }
}