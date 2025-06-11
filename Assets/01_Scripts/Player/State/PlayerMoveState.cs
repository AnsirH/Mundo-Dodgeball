using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mundo_dodgeball.Player.StateMachine
{
    public class PlayerMoveState : PlayerStateBase
    {
        public PlayerMoveState(IPlayerContext playerContext) : base(playerContext)
        {
            movement = playerContext.Movement;
        }

        public override void EnterState(StateTransitionInputData inputData)
        {
            playerContext.Anim.SetBool("IsMove", true);

            Vector3 targetPoint = inputData.mousePosition;
            targetPoint.y = 0.0f;
            
            movement.SetMovementTarget(targetPoint);
        }

        public override void ExitState()
        {
            playerContext.Anim.SetBool("IsMove", false);
        }

        public override void UpdateState()
        {
            //if (playerController.Attack.AttackTrigger)
            //    playerController.StateMachine.ChangeState(EPlayerState.Attack);

            //else if (PlayerController.PM.IsMoving == false)
            //    PlayerController.StateMachine.ChangeState(EPlayerState.Idle);

        }

        public override void NetworkUpdateState(float runnerDeltaTime)
        {
            if (!movement.IsArrived)
            {
                movement.MoveTowardTarget(playerContext.Runner.DeltaTime);
            }
            else
            {
                movement.CompleteMove();
                playerContext.ChangeState(EPlayerState.Idle);
            }
        }

        private PlayerMovement movement;
    }
}