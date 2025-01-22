using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerCharacterControl
{
    public class PlayerAnimEventHandler : MonoBehaviour
    {
        UnityEvent<string> onAnimationEventActions = new();
        public UnityEvent<string> OnAnimationEventActions => onAnimationEventActions;

        public void SendEventTag(string tag)
        {
            onAnimationEventActions.Invoke(tag);
        }
    }
}