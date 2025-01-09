using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.State
{
    public class PlayerStateMachine
    {
        // ���� ��ȯ
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

        // ���� ���·� ��ȯ
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