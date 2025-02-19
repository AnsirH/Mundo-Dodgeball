using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerCharacterControl
{
    public class PlayerAnimEventHandler : MonoBehaviourPunCallbacks
    {
        UnityEvent<string> onAnimationEventActions = new();
        public UnityEvent<string> OnAnimationEventActions => onAnimationEventActions;

        public void SendEventTag(string tag)
        {
            if (!photonView.IsMine) return;
            photonView.RPC("SendEventTagRPC", RpcTarget.All, tag);
        }

        [PunRPC]
        private void SendEventTagRPC(string tag)
        {
            onAnimationEventActions.Invoke(tag);
        }
    }
}