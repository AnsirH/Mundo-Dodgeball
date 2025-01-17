using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    public class PlayerMoveState : PlayerStateBase
    {
        public PlayerMoveState(PlayerController playerController) : base(playerController)
        {
        }

        public override void EnterState()
        {
            Debug.Log($"{GetType().Name} 상태 진입");
            playerController.Anim.SetBool("IsMove", true);
        }

        public override void ExitState()
        {
            Debug.Log($"{GetType().Name} 상태 종료");
            playerController.Anim.SetBool("IsMove", false);
        }

        public override void UpdateState()
        {
            if (playerController.Attack.CheckAttack())
                playerController.StateMachine.ChangeState(EPlayerState.Attack);

            if (PlayerController.Movement.IsMove == false)
                PlayerController.StateMachine.ChangeState(EPlayerState.Idle);

        }
    }
}