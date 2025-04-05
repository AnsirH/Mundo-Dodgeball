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
            // 이전 상태가 이동 상태인지 확인 및 저장
            //isPrevStateIsMove = context.StateMachine.PrevState is PlayerMoveState;

            // 이전 상태가 이동이라면 이동 중지
            //if (isPrevStateIsMove)
            //    playerController.PM.StopMove();

            //// [RPC] 공격 실행
            //attack.photonView.RPC("StartAttack", Photon.Pun.RpcTarget.All);

            //// 공격 애니메이션 실행
            //playerController.Anim.SetTrigger("Attack"); // 상수로 변경할 것
        }

        public override void ExitState()
        {
            //// 이전 상태가 이동 상태였을 때 마저 이동
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
            //        // 이전 상태에 따른 상태 전환
            //        playerController.StateMachine.ChangeState(isPrevStateIsMove ? EPlayerState.Move : EPlayerState.Idle);
            //    }
            //}
        }

        // 이전 상태가 이동이었는지 저장하는 변수
        bool isPrevStateIsMove = false;
        private readonly string AttackAnimationName = "Attack03_End";

        private IPlayerComponent attack;

    }
}