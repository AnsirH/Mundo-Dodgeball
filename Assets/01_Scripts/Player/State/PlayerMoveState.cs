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
            Debug.Log($"{GetType().Name} ���� ����");
        }

        public override void ExitState()
        {
            Debug.Log($"{GetType().Name} ���� ����");
        }

        public override void UpdateState()
        {
            if (PlayerController.Movement.IsMove == false)
                PlayerController.StateMachine.ChangeState(EPlayerState.Idle);
        }
    }
}