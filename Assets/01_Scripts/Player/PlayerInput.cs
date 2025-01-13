using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl
{
    public class PlayerInput
    {
        public void Updated()
        {
            if (!IsPressedQ && Input.GetKeyDown(KeyCode.Q))
            {
                isPressedQ = true;
            }
        }

        private bool isPressedQ = false;

        public bool IsPressedQ => isPressedQ;
    }
}
