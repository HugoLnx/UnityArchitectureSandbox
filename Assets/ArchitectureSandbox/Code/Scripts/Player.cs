using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ArchitectureSandbox
{
    [RequireComponent(typeof(DirectionableComponent))]
    public class Player : MonoBehaviour
    {
        private DirectionableComponent _directionable;

        private void Awake()
        {
            _directionable = GetComponent<DirectionableComponent>();
        }

        private void Update()
        {
            ProcessRotate(Input.GetAxis("Horizontal"));
        }

        private void ProcessRotate(float xInput)
        {
            if (xInput == 0) _directionable.RotationStop();
            else if (xInput > 0) _directionable.ClockwiseRotationStart();
            else _directionable.AntiClockwiseRotationStart();
        }
    }
}