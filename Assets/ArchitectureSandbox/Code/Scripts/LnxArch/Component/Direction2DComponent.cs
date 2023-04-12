using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace ArchitectureSandbox.LnxArchSandbox
{
    public class Direction2DComponent : LnxValueComponentSynchronized<Vector2>
    {
        private Transform Transform => _entity.transform;

        protected override Vector2 PlainValue {
            get => ToXZ(Transform.forward);
            set => Transform.forward = ToX0Y(value);
        }

        private static Vector3 ToX0Y(Vector2 v) => new(v.x, 0f, v.y);
        private static Vector3 ToX0Y(Vector3 v) => ToX0Y(v);
        private static Vector2 ToXZ(Vector3 v) => new(v.x, v.z);
    }
}