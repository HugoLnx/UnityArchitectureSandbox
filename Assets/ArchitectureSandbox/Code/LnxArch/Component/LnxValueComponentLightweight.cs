using System;
using System.Collections;

namespace LnxArch
{
    public abstract class LnxValueComponentLightweight<T> : LnxBehaviour, ILnxValueComponentLightweight<T>
    {
        protected T _value;
        public virtual T Value {
            get => _value;
            set => _value = value;
        }
    }
}