using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    public class PlayerAttakState : PlayerStateBase
    {
        public PlayerAttakState(PlayerController playerController) : base(playerController)
        {
            AnimationClip[] animationClips = playerController.Anim.runtimeAnimatorController.animationClips;
            for (int i = 0; i < animationClips.Length; i++)
            {
                if (animationClips[i].name == AttackAnimationName)
                {
                    attackAnimationActionTime = animationClips[i].events[0].time;
                }
            }            
        }

        public override void EnterState()
        {
            Debug.Log($"{GetType().Name} 상태 진입");

            if (playerController.StateMachine.PrevState is PlayerMoveState)
            {
                playerController.Movement.enabled = false;
                isMoving = true;
            }

            playerController.Attack.StartAttack();

            playerController.Anim.SetTrigger("Attack"); // 상수로 변경할 것.
        }

        public override void ExitState()
        {
            Debug.Log($"{GetType().Name} 상태 종료");
            if (isMoving)
            {
                playerController.Movement.enabled = true;
                isMoving = false;
            }

            doAnimationAction = false;
        }

        public override void UpdateState()
        {
            if (playerController.Anim.GetCurrentAnimatorStateInfo(0).IsName(AttackAnimationName))
            {
                if (playerController.Anim.IsInTransition(0))
                {
                    playerController.StateMachine.ChangeState(isMoving ? EPlayerState.Move : EPlayerState.Idle);
                }

                else if (playerController.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime > attackAnimationActionTime && !doAnimationAction)
                {
                    doAnimationAction = true;
                    Debug.Log("도끼 던져");
                }
            }
        }

        bool isMoving = false;
        private readonly string AttackAnimationName = "Attack03_End";
        private float attackAnimationActionTime = 0.0f;
        private bool doAnimationAction = false;
    }
}