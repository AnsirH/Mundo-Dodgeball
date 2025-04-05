using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    public class PlayerMoveState : PlayerStateBase
    {
        public PlayerMoveState(IPlayerContext playerContext, IPlayerAction movement) : base(playerContext)
        {
            this.movement = movement;
        }

        public override void EnterState()
        {
            context.Anim.SetBool("IsMove", true);
            movement.ExecuteAction();
        }

        public override void ExitState()
        {
            context.Anim.SetBool("IsMove", false);
        }

        public override void UpdateState()
        {
            //if (playerController.Attack.AttackTrigger)
            //    playerController.StateMachine.ChangeState(EPlayerState.Attack);

            //else if (PlayerController.PM.IsMoving == false)
            //    PlayerController.StateMachine.ChangeState(EPlayerState.Idle);

        }

        private IPlayerAction movement;
    }
}