using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchitectureSandbox.Atoms
{
    public class PlayerInterfaces : MonoBehaviour
    {
        private const string HorizontalAxisName = "Horizontal";
        private IRotatable2D _rotatable;

        private void Awake()
        {
            _rotatable = GetComponent<IRotatable2D>();
        }

        private void Update()
        {
            ProcessRotate(Input.GetAxis(HorizontalAxisName));
        }

        private void ProcessRotate(float xInput)
        {
            if (xInput == 0) _rotatable.RotationStop();
            else if (xInput > 0) _rotatable.ClockwiseRotationStart();
            else _rotatable.AntiClockwiseRotationStart();
        }
    }
}