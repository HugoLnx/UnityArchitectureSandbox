using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using TMPro;
using UnityEngine;

namespace ArchitectureSandbox.LnxArchSandbox
{
    public class Direction2DUI : LnxBehaviour
    {
        private TMP_Text _text;
        private Direction2DComponent _direction;


        [AutoFetch]
        private void Prepare(TMP_Text text, Direction2DComponent direction)
        {
            _text = text;
            _direction = direction;
            _direction.OnChange += (_, _, _) => UpdateText();
        }

        private void UpdateText()
        {
            float signedAngle = Vector2.SignedAngle(Vector2.right, _direction.Value);
            float angle = ((signedAngle % 360f) + 360f) % 360f;
            _text.text = angle.ToString();
        }
    }
}