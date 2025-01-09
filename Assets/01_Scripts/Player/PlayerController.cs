using Player.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerStateMachine playerStateMachine;

        void Start()
        {
            playerStateMachine = new(this);
            playerStateMachine.ChangeState(EPlayerState.Idle);
        }

        void Update()
        {
            playerStateMachine.Updated();
        }
    }
}