using System;
using System.Collections.Generic;
using UnityEngine;

namespace LnxArch
{
    public interface IFetchAttribute
    {
        int Order { get; set; }
        Component FetchOne(MonoBehaviour behaviour, LnxEntity entity, Type type);
        IEnumerable<Component> FetchMany(MonoBehaviour behaviour, LnxEntity entity, Type type);
    }

}