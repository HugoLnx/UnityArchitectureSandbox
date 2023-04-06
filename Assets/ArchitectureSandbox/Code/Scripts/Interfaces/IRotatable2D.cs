using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchitectureSandbox.Atoms
{
    public interface IRotatable2D
    {
        void ClockwiseRotationStart();
        void AntiClockwiseRotationStart();
        void RotationStop();
    }
}