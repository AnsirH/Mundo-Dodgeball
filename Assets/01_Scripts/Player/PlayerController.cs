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
            playerStateMachine.ChangeState(EPlayerState.Idle); // Idle 상태로 초기화
        }

        void Update()
        {
            playerStateMachine.Updated();
        }
    }
}