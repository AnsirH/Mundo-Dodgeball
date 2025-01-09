using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.State
{
    public class PlayerStateMachine
    {
        // 상태 전환
        public void ChangeState(PlayerStateBase newState)
        {
            if (currentState != null)
            {
                prevState = currentState;
                prevState.ExitState();
            }

            currentState = newState;
            currentState.EnterState();
        }

        PlayerStateBase currentState;

        // 이전 상태로 전환
        public void UndoState()
        {
            if (prevState != null)
            {
                ChangeState(prevState);
            }
        }

        PlayerStateBase prevState;

        public void Updated()
        {
            currentState?.UpdateState();
        }
    }
}