using UnityEngine;

namespace ArchitectureSandbox.LnxArchSandbox
{
    public class PlayerInput
    {
        private const string HorizontalAxisName = "Horizontal";
        public float AxisX => Input.GetAxis(HorizontalAxisName);
    }
}