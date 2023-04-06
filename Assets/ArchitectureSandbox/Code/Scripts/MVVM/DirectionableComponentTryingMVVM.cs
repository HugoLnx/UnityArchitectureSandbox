using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchitectureSandbox
{
    public class Directioned2DView
    {
        private Transform _transform;
        private IDirectioned2D _directioned;
        // public Vector2 Direction {
        //     get => toXZ(_transform.forward);
        //     set => _transform.forward = toX0Y(value);
        // }

        public Directioned2DView(Transform transform)
        {
            _transform = transform;
        }

        public Directioned2DView Mirror(IDirectioned2D directioned)
        {
            _directioned = directioned;
            _directioned.OnChangeDirection += (_) => this.Update();
            return this;
        }

        private void Update()
        {
            _transform.forward = toX0Y(_directioned.Direction);
        }

        private Vector3 toX0Y(Vector2 v) => new(v.x, 0f, v.y);
        private Vector3 toX0Y(Vector3 v) => toX0Y(v);
        private Vector2 toXZ(Vector3 v) => new(v.x, v.z);
    }

    public interface IDirectioned2D
    {
        event Action<Vector2> OnChangeDirection;

        Vector2 Direction {get; set;}
    }

    public class Directioned2D : IDirectioned2D
    {
        private Vector2 _direction = Vector2.up;

        public Vector2 Direction {
            get => _direction;
            set {
                _direction = value;
                OnChangeDirection(_direction);
            }
        }

        public event Action<Vector2> OnChangeDirection;
    }

    public interface ITimedRotatable2D
    {
        void TickClockwise(float deltaTime);
        void TickAntiClockwise(float deltaTime);
        void TickInactive(float deltaTime);
    }

    public class TimedLinearRotatable2D : ITimedRotatable2D
    {
        private readonly IDirectioned2D _directioned;
        private readonly float _speed;

        public TimedLinearRotatable2D(IDirectioned2D directioned, float speed)
        {
            _directioned = directioned;
            _speed = speed;
        }

        public void TickClockwise(float deltaTime)
        {
            TickAntiClockwise(-deltaTime);
        }

        public void TickAntiClockwise(float deltaTime)
        {
            float angle = _speed * deltaTime;
            _directioned.Direction = Quaternion.AngleAxis(angle, Vector3.forward) * _directioned.Direction;
        }

        public void TickInactive(float deltaTime)
        {}
    }

    public class TimedAcceleratedRotatable2D : ITimedRotatable2D
    {
        private readonly IDirectioned2D _directioned;
        private readonly float _maxSpeed;
        private readonly float _acceleration;
        private readonly float _deacceleration;
        private float _speed;

        public TimedAcceleratedRotatable2D(IDirectioned2D directioned, float maxSpeed, float acceleration, float deacceleration)
        {
            _directioned = directioned;
            _maxSpeed = maxSpeed;
            _acceleration = acceleration;
            _deacceleration = deacceleration;
            _speed = 0f;
        }

        public void TickClockwise(float deltaTime)
        {
            TickAntiClockwise(-deltaTime);
        }

        public void TickAntiClockwise(float deltaTime)
        {
            ApplySpeed(deltaTime);
            Accelerate(deltaTime);
        }

        public void TickInactive(float deltaTime)
        {
            ApplySpeed(deltaTime);
            Deaccelerate(deltaTime);
        }

        private void ApplySpeed(float deltaTime)
        {
            float angle = _speed * deltaTime;
            _directioned.Direction = Quaternion.AngleAxis(angle, Vector3.forward) * _directioned.Direction;
        }

        private void Accelerate(float deltaTime)
        {
            _speed = Mathf.Min(_maxSpeed, _speed + deltaTime * _acceleration);
        }

        private void Deaccelerate(float deltaTime)
        {
            _speed = Mathf.Max(0, _speed - deltaTime * _deacceleration);
        }
    }

    public class StateTicker<T>
    where T : Enum
    {
        public delegate void TickCallback(float deltaTime);
        private Dictionary<T, TickCallback> _callbacks = new();
        private T _state;

        public StateTicker(T initialState = default)
        {
            _state = initialState;
        }

        public void SetState(T state)
        {
            _state = state;
        }

        public void Tick(float deltaTime)
        {
            _callbacks?[_state]?.Invoke(deltaTime);
        }

        public void Subscribe(T state, TickCallback callback)
        {
            if (_callbacks.ContainsKey(state))
            {
                _callbacks[state] += callback;
            }
            else
            {
                _callbacks[state] = callback;
            }
        }
    }

    public static class Rotatable2DTickerBuilder
    {
        public enum State {Inactive, Clockwise, AntiClockwise}
        public static StateTicker<State> Build(ITimedRotatable2D rotatable, State start)
        {
            StateTicker<State> ticker = new(start);
            ticker.Subscribe(State.Clockwise, rotatable.TickClockwise);
            ticker.Subscribe(State.AntiClockwise, rotatable.TickAntiClockwise);
            ticker.Subscribe(State.Inactive, rotatable.TickInactive);
            return ticker;
        }
    }

    public class DirectionableComponentTryingMVVM : MonoBehaviour
    {
        [SerializeField] private float _speed;
        private Directioned2DView _view;
        private IDirectioned2D _directioned;
        private ITimedRotatable2D _rotatable;
        private StateTicker<Rotatable2DTickerBuilder.State> _stateTicker;

        private void Awake()
        {
            _directioned = new Directioned2D(); // Model
            _view = new Directioned2DView(transform) // ViewModel
                .Mirror(_directioned);

            _rotatable = new TimedLinearRotatable2D(_directioned, _speed);
            _stateTicker = Rotatable2DTickerBuilder.Build(_rotatable, start: Rotatable2DTickerBuilder.State.Inactive);
        }

        private void Update()
        {
            _stateTicker.Tick(Time.deltaTime);
        }

        public void ClockwiseRotationStart() => _stateTicker.SetState(Rotatable2DTickerBuilder.State.Clockwise);
        public void AntiClockwiseRotationStart() => _stateTicker.SetState(Rotatable2DTickerBuilder.State.AntiClockwise);
        public void RotationStop() => _stateTicker.SetState(Rotatable2DTickerBuilder.State.Inactive);
    }
}