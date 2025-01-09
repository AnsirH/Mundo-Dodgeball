using Player.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerStateBase[] states = new PlayerStateBase[4];
        private PlayerStateMachine playerStateMachine = new();

        void Start()
        {
            states[(int)EPlayerState.Idle] = new PlayerIdleState(this);
            states[(int)EPlayerState.Move] = new PlayerMoveState(this);
            states[(int)EPlayerState.Attack] =  new PlayerAttachState(this);
            states[(int)EPlayerState.Die] = new PlayerDieState(this);
        }

        // Update is called once per frame
        void Update()
        {
            playerStateMachine.Updated();
        }
    }
}