using LnxArch;
using UnityEngine;

namespace ArchitectureSandbox.LnxArchSandbox
{
    public class Direction2DTransformSynchronizer : LnxComponentSynchronizer<Vector2, Direction2DComponent>
    {
        private Transform Transform => _entity.transform;

        protected override Vector2 Pull()
        {
            return ToXZ(Transform.forward);
        }

        protected override void Push(Vector2 value)
        {
            Transform.forward = ToX0Y(value);
        }

        private static Vector3 ToX0Y(Vector2 v) => new(v.x, 0f, v.y);
        private static Vector3 ToX0Y(Vector3 v) => ToX0Y(v);
        private static Vector2 ToXZ(Vector3 v) => new(v.x, v.z);
    }
}