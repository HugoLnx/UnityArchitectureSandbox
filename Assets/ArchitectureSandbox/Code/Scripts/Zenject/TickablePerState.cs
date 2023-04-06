using System;

namespace ArchitectureSandbox.Zen
{
    public class TickablePerState<T>
    where T : Enum
    {
        public T State { get; private set; }

        private Action<float> _tickAction;
        private Func<T, Action<float>> _mapToTick;

        public TickablePerState(Func<T, Action<float>> mapToTick, T startWith = default)
        {
            _mapToTick = mapToTick;
            StartTickingFor(startWith);
        }

        public void Tick(float deltaTime)
        {
            _tickAction?.Invoke(deltaTime);
        }

        public void StartTickingFor(T state)
        {
            State = state;
            _tickAction = _mapToTick(state);
        }
    }
}