using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchitectureSandbox.LnxArchSandbox
{
    public static class Math2D
    {
        public static Vector2 Rotate(Vector2 vector, float byDegrees)
        {
            return Quaternion.AngleAxis(byDegrees, Vector3.forward) * vector;
        }
    }
}