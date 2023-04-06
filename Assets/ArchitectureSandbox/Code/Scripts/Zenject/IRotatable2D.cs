using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchitectureSandbox.Zen
{
    public enum RotationState {
        NoRotation,
        RotatingClockwise,
        RotatingAntiClockwise
    }
    public interface IRotatable2D
    {
        RotationState RotationState {get; set;}
    }
}