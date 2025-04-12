using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    public class PlayerAttackState : PlayerStateBase
    {
        public PlayerAttackState(IPlayerContext playerContext, IPlayerAction attack) : base(playerContext)
        {
            this.attack = attack;
        }

        public override void EnterState()
        {
            attack.ExecuteAction();
            context.Anim.SetTrigger("Attack");
        }

        public override void ExitState()
        {
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

        private IPlayerAction attack;

    }
}