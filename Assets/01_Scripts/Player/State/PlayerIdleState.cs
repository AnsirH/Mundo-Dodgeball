using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mundo_dodgeball.Player.StateMachine
{
    public class PlayerIdleState : PlayerStateBase
    {
        public PlayerIdleState(IPlayerContext playerContext) : base(playerContext)
        {
        }

        public override void EnterState(StateTransitionInputData inputData)
        {
            Debug.Log($"{GetType().Name} 상태 진입");
        }

        public override void ExitState()
        {
        }

        public override void NetworkUpdateState(float runnerDeltaTime)
        {
        }

        public override void UpdateState()
        {
            //if (playerController.Attack.AttackTrigger)
            //    playerController.StateMachine.ChangeState(EPlayerState.Attack);

            //if (playerController.PM.IsMoving)
            //    playerController.StateMachine.ChangeState(EPlayerState.Move);
        }
    }
}