using System;
using System.Collections.Generic;
using UnityEngine;

namespace LnxArch
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromLocalChildAttribute : Attribute, IFetchAttribute
    {
        public int Order { get; set; }

        public FromLocalChildAttribute(int order = 0)
        {
            Order = order;
        }

        public Component FetchOne(MonoBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponentInChildren(type, includeInactive: true);
        }
        public IEnumerable<Component> FetchMany(MonoBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponentsInChildren(type, includeInactive: true);
        }
    }

}