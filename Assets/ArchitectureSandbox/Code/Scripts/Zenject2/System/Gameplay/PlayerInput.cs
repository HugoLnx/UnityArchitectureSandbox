using UnityEngine;

namespace ArchitectureSandbox.Zen2
{
    public class PlayerInput
    {
        private const string HorizontalAxisName = "Horizontal";
        public float AxisX => Input.GetAxis(HorizontalAxisName);
    }
}