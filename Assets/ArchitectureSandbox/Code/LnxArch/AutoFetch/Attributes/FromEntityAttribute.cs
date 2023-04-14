using System;
using System.Collections.Generic;
using UnityEngine;

namespace LnxArch
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FromEntityAttribute : Attribute, IFetchAttribute
    {
        public int Order { get; set; }

        public FromEntityAttribute(int order = 0)
        {
            this.Order = order;
        }

        public Component FetchOne(MonoBehaviour _, LnxEntity entity, Type type)
        {
            return entity.FetchFirst(type);
        }
        public IEnumerable<Component> FetchMany(MonoBehaviour _, LnxEntity entity, Type type)
        {
            return entity.FetchAll(type);
        }
    }

}