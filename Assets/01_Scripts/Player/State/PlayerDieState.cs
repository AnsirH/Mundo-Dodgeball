using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    public class PlayerDieState : PlayerStateBase
    {
        public PlayerDieState(PlayerController playerController) : base(playerController)
        {
        }

        public override void EnterState()
        {
            Debug.Log($"{GetType().Name} 상태 진입");
        }

        public override void ExitState()
        {
            Debug.Log($"{GetType().Name} 상태 종료");
        }

        public override void UpdateState()
        {
            //Debug.Log($"{GetType().Name} 상태 실행 중");
        }
    }
}