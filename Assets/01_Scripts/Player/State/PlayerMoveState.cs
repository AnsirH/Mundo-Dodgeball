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

            if (!movement.TrySetMovementTarget(targetPoint))
                playerContext.ChangeState(EPlayerState.Idle);

        }

        public override void ExitState()
        {
            playerContext.Anim.SetBool("IsMove", false);
        }

        public override void UpdateState()
        {
            if (movement.IsArrived)
            {
                movement.CompleteMove();
                playerContext.ChangeState(EPlayerState.Idle);
            }
        }

        public override void NetworkUpdateState(float runnerDeltaTime)
        {
        }

        private PlayerMovement movement;
    }
}