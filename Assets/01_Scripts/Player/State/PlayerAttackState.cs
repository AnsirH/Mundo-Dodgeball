using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    public class PlayerAttackState : PlayerStateBase
    {
        public PlayerAttackState(IPlayerContext playerContext, IPlayerComponent attack) : base(playerContext)
        {
            this.attack = attack;
        }

        public override void EnterState()
        {
            // ���� ���°� �̵� �������� Ȯ�� �� ����
            //isPrevStateIsMove = context.StateMachine.PrevState is PlayerMoveState;

            // ���� ���°� �̵��̶�� �̵� ����
            //if (isPrevStateIsMove)
            //    playerController.PM.StopMove();

            //// [RPC] ���� ����
            //attack.photonView.RPC("StartAttack", Photon.Pun.RpcTarget.All);

            //// ���� �ִϸ��̼� ����
            //playerController.Anim.SetTrigger("Attack"); // ����� ������ ��
        }

        public override void ExitState()
        {
            //// ���� ���°� �̵� ���¿��� �� ���� �̵�
            //if (isPrevStateIsMove)
            //{
            //    playerController.PM.ContinueMoveToTarget();
            //    isPrevStateIsMove = false;
            //}
        }

        public override void UpdateState()
        {
            //if (playerController.Anim.GetCurrentAnimatorStateInfo(0).IsName(AttackAnimationName))
            //{
            //    if (playerController.Anim.IsInTransition(0))
            //    {
            //        // ���� ���¿� ���� ���� ��ȯ
            //        playerController.StateMachine.ChangeState(isPrevStateIsMove ? EPlayerState.Move : EPlayerState.Idle);
            //    }
            //}
        }

        // ���� ���°� �̵��̾����� �����ϴ� ����
        bool isPrevStateIsMove = false;
        private readonly string AttackAnimationName = "Attack03_End";

        private IPlayerComponent attack;

    }
}