using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace ArchitectureSandbox.LnxArchSandbox
{
    public class Directionable2DComponent : LnxBehaviour
    {
        private Transform _transform;

        public Vector2 Direction {
            get => ToXZ(_transform.forward);
            set => _transform.forward = ToX0Y(value);
        }

        [AutoFetch]
        private void Prepare()
        {
            _transform = _entity.transform;
        }

        public void RotateBy(float angle)
        {
            Direction = Quaternion.AngleAxis(angle, Vector3.forward) * Direction;
        }
        private static Vector3 ToX0Y(Vector2 v) => new(v.x, 0f, v.y);
        private static Vector3 ToX0Y(Vector3 v) => ToX0Y(v);
        private static Vector2 ToXZ(Vector3 v) => new(v.x, v.z);
    }
}