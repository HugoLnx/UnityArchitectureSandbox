using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchitectureSandbox.Zen2
{
    public class Directionable2D : MonoBehaviour
    {
        public Vector2 Direction {
            get => ToXZ(transform.forward);
            set => transform.forward = ToX0Y(value);
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