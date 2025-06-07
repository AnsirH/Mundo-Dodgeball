using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerCharacterControl
{
    public class PlayerAnimEventHandler : NetworkObject
    {
        UnityEvent<string> onAnimationEventActions = new();
        public UnityEvent<string> OnAnimationEventActions => onAnimationEventActions;

        public void SendEventTag(string tag)
        {
            if (!HasStateAuthority) return;
            //photonView.RPC("SendEventTagRPC", RpcTarget.All, tag);
        }
    }
}