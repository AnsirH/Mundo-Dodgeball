using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mundo_dodgeball.Player.StateMachine
{
    public struct StateTransitionInputData
    {
        public Vector3 mousePosition;

        public StateTransitionInputData(Vector3 mousePosition)
        {
            this.mousePosition = mousePosition;
        }
    }

    // 플레이어 상태 베이스
    public abstract class PlayerStateBase
    {
        public PlayerStateBase(IPlayerContext playerContext)
        {
            this.playerContext = playerContext;
        }

        protected IPlayerContext playerContext;

        // 상태 진입 시 호출
        public abstract void EnterState(StateTransitionInputData inputData = new());

        // 상태 종료 시 호출
        public abstract void ExitState();

        // 상태 업데이트. PlayerController Update에서 호출
        public abstract void UpdateState();

        // 상태 업데이트. PlayerController NetworkFixedUpdate에서 호출
        public abstract void NetworkUpdateState(float runnerDeltaTime);
    }
}