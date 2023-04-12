using System;
using System.Collections;

namespace LnxArch
{
    public abstract class LnxComponentLightweight<T> : LnxBehaviour, ILnxComponentLightweight<T>
    {
        protected T _value;
        public virtual T Value {
            get => _value;
            set => _value = value;
        }
    }
}