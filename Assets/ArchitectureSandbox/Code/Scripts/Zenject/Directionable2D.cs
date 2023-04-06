using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchitectureSandbox.Zen
{
    public class Directionable2D : MonoBehaviour
    {
        public Vector2 Direction {
            get => toXZ(transform.forward);
            set => transform.forward = toX0Y(value);
        }

        public void RotateBy(float angle)
        {
            Direction = Quaternion.AngleAxis(angle, Vector3.forward) * Direction;
        }
        private Vector3 toX0Y(Vector2 v) => new(v.x, 0f, v.y);
        private Vector3 toX0Y(Vector3 v) => toX0Y(v);
        private Vector2 toXZ(Vector3 v) => new(v.x, v.z);
    }
}