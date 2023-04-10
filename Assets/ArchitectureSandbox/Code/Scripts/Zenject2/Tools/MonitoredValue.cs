using System;
using System.Collections.Generic;

namespace ArchitectureSandbox.Zen2
{
    public class MonitoredValue<T>
    {
        public T Value { get; private set; }
        public bool HasChanged { get; private set; }
        public bool HasIncreased { get; private set; }
        public bool HasDecreased { get; private set; }

        public delegate void ChangeCallback(T newValue, T oldValue);
        public event ChangeCallback OnChange;
        public event ChangeCallback OnIncrease;
        public event ChangeCallback OnDecrease;

        public MonitoredValue(T start)
        {
            Update(start);
        }

        public bool Update(T newValue)
        {
            T oldValue = Value;
            Value = newValue;
            UpdateComparisonProperties(newValue, oldValue);
            TriggerComparisonEvents(newValue, oldValue);
            return HasChanged;
        }

        private void UpdateComparisonProperties(T newValue, T oldValue)
        {
            int comparison = Comparer<T>.Default.Compare(oldValue, newValue);
            HasChanged = comparison != 0;
            HasIncreased = comparison > 0;
            HasDecreased = comparison < 0;
        }

        private void TriggerComparisonEvents(T newValue, T oldValue)
        {
            if (HasChanged) OnChange?.Invoke(newValue, oldValue);
            if (HasIncreased) OnIncrease?.Invoke(newValue, oldValue);
            if (HasDecreased) OnDecrease?.Invoke(newValue, oldValue);
        }
    }
}