using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerCharacterControl.State
{
    // 플레이어 상태 베이스
    public abstract class PlayerStateBase
    {
        public PlayerStateBase(IPlayerContext playerContext)
        {
            this.context = playerContext;
        }

        protected IPlayerContext context;

        // 상태 진입 시 호출
        public abstract void EnterState();

        // 상태 종료 시 호출
        public abstract void ExitState();

        // 상태 업데이트. PlayerController Update에서 호출
        public abstract void UpdateState();
    }
}