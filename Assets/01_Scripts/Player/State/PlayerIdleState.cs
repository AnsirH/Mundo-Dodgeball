using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    public class PlayerIdleState : PlayerStateBase
    {
        public PlayerIdleState(PlayerController playerController) : base(playerController)
        {
        }

        public override void EnterState()
        {
            Debug.Log($"{GetType().Name} ���� ����");
        }

        public override void ExitState()
        {
            Debug.Log($"{GetType().Name} ���� ����");
        }

        public override void UpdateState()
        {
            if (playerController.Attack.CheckAttack())
                playerController.StateMachine.ChangeState(EPlayerState.Attack);

            if (playerController.Movement.IsMove)
                playerController.StateMachine.ChangeState(EPlayerState.Move);
        }
    }
}