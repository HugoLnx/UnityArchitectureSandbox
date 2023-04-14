using System;
using System.Collections.Generic;
using UnityEngine;

namespace LnxArch
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromLocalAncestorAttribute : Attribute, IFetchAttribute
    {
        public int Order { get; set; }

        public FromLocalAncestorAttribute(int order = 0)
        {
            Order = order;
        }

        public Component FetchOne(MonoBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponentInParent(type, includeInactive: true);
        }
        public IEnumerable<Component> FetchMany(MonoBehaviour behaviour, LnxEntity _, Type type)
        {
            return behaviour.GetComponentsInParent(type, includeInactive: true);
        }
    }

}