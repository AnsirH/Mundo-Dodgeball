using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    // 플레이어 상태 베이스
    public abstract class PlayerStateBase
    {
        public PlayerStateBase(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        protected PlayerController playerController;

        public PlayerController PlayerController => playerController;

        // 상태 진입 시 호출
        public abstract void EnterState();

        // 상태 종료 시 호출
        public abstract void ExitState();

        // 상태 업데이트. PlayerController Update에서 호출
        public abstract void UpdateState();
    }
}