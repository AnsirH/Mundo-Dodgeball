using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    public class PlayerIdleState : PlayerStateBase
    {
        public PlayerIdleState(IPlayerContext playerContext) : base(playerContext)
        {
        }

        public override void EnterState()
        {
            Debug.Log($"{GetType().Name} ���� ����");
        }

        public override void ExitState()
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