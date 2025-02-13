using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    public class PlayerAttakState : PlayerStateBase
    {
        public PlayerAttakState(PlayerController playerController) : base(playerController)
        {
        }

        public override void EnterState()
        {
            Debug.Log($"{GetType().Name} ���� ����");

            if (playerController.StateMachine.PrevState is PlayerMoveState)
            {
                playerController.Movement.enabled = false;
                isMoving = true;
            }

            playerController.Attack.StartAttack();

            playerController.Anim.SetTrigger("Attack"); // ����� ������ ��
        }

        public override void ExitState()
        {
            Debug.Log($"{GetType().Name} ���� ����");
            if (isMoving)
            {
                playerController.Movement.enabled = true;
                isMoving = false;
            }
        }

        public override void UpdateState()
        {
            if (playerController.Anim.GetCurrentAnimatorStateInfo(0).IsName(AttackAnimationName))
            {
                if (playerController.Anim.IsInTransition(0))
                {
                    playerController.StateMachine.ChangeState(isMoving ? EPlayerState.Move : EPlayerState.Idle);
                }
            }
        }

        bool isMoving = false;
        private readonly string AttackAnimationName = "Attack03_End";
    }
}