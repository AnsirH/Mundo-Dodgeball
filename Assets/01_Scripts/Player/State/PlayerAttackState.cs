using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mundo_dodgeball.Player.StateMachine
{
    public class PlayerAttackState : PlayerStateBase
    {
        public PlayerAttackState(IPlayerContext playerContext) : base(playerContext)
        {
            attack = playerContext.Attack;
        }

        public override void EnterState(StateTransitionInputData inputData)
        {
            Vector3 targetPoint = inputData.mousePosition;
            targetPoint.y = 0.0f;
            attack.StartAttack(targetPoint);
        }
        public override void ExitState()
        {
        }

        public override void UpdateState()
        {
        }

        public override void NetworkUpdateState(float runnerDeltaTime)
        {
            if (!attack.IsRotationComplete())
            {
                attack.RotateTowardsTarget();
            }

            if (!attack.Attacking)
            {
                attack.StartCoolDown(5.0f);
                playerContext.ChangeState(EPlayerState.Idle);
            }
        }

        private PlayerAttack attack;
    }
}