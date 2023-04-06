using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace ArchitectureSandbox.Atoms
{
    public class PlayerAtoms : MonoBehaviour
    {
        [SerializeField] private VoidBaseEventReference _rotationClockwiseStart;
        [SerializeField] private VoidBaseEventReference _rotationAntiClockwiseStart;
        [SerializeField] private VoidBaseEventReference _rotationStop;
        private const string HorizontalAxisName = "Horizontal";
        private void Update()
        {
            ProcessRotate(Input.GetAxis(HorizontalAxisName));
        }

        private void ProcessRotate(float xInput)
        {
            if (xInput == 0) _rotationStop.Event.Raise();
            else if (xInput > 0) _rotationClockwiseStart.Event.Raise();
            else _rotationAntiClockwiseStart.Event.Raise();
        }
    }
}