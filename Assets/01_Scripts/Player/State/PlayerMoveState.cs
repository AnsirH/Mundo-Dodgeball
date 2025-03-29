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
            playerController.Anim.SetBool("IsMove", true);
        }

        public override void ExitState()
        {
            playerController.Anim.SetBool("IsMove", false);
        }

        public override void UpdateState()
        {
            if (playerController.Attack.AttackTrigger)
                playerController.StateMachine.ChangeState(EPlayerState.Attack);

            else if (PlayerController.PM.IsMoving == false)
                PlayerController.StateMachine.ChangeState(EPlayerState.Idle);

        }
    }
}