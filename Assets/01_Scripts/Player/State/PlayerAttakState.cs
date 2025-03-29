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
            // ���� ���°� �̵� �������� Ȯ�� �� ����
            isPrevStateIsMove = playerController.StateMachine.PrevState is PlayerMoveState;

            // ���� ���°� �̵��̶�� �̵� ����
            if (isPrevStateIsMove)
                playerController.PM.StopMove();

            // [RPC] ���� ����
            playerController.Attack.photonView.RPC("StartAttack", Photon.Pun.RpcTarget.All);

            // ���� �ִϸ��̼� ����
            playerController.Anim.SetTrigger("Attack"); // ����� ������ ��
        }

        public override void ExitState()
        {
            // ���� ���°� �̵� ���¿��� �� ���� �̵�
            if (isPrevStateIsMove)
            {
                playerController.PM.ContinueMoveToTarget();
                isPrevStateIsMove = false;
            }
        }

        public override void UpdateState()
        {
            if (playerController.Anim.GetCurrentAnimatorStateInfo(0).IsName(AttackAnimationName))
            {
                if (playerController.Anim.IsInTransition(0))
                {
                    // ���� ���¿� ���� ���� ��ȯ
                    playerController.StateMachine.ChangeState(isPrevStateIsMove ? EPlayerState.Move : EPlayerState.Idle);
                }
            }
        }

        // ���� ���°� �̵��̾����� �����ϴ� ����
        bool isPrevStateIsMove = false;
        private readonly string AttackAnimationName = "Attack03_End";
    }
}