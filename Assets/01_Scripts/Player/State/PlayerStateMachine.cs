using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    public class PlayerStateMachine
    {
        public PlayerStateMachine(PlayerController playerController)
        {
            states[(int)EPlayerState.Idle] = new PlayerIdleState(playerController);
            states[(int)EPlayerState.Move] = new PlayerMoveState(playerController);
            states[(int)EPlayerState.Attack] = new PlayerAttachState(playerController);
            states[(int)EPlayerState.Die] = new PlayerDieState(playerController);
        }

        private PlayerStateBase[] states = new PlayerStateBase[4];

        // ���� ��ȯ
        public void ChangeState(EPlayerState newState)
        {
            if (newState == EPlayerState.None) return;

            _ChangeState(states[(int)newState]);
        }

        private void _ChangeState(PlayerStateBase newState)
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
                _ChangeState(prevState);
            }
        }

        PlayerStateBase prevState;

        public void Updated()
        {
            currentState?.UpdateState();
        }
    }
}