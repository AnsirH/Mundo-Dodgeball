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
            states[(int)EPlayerState.Attack] = new PlayerAttakState(playerController);
            states[(int)EPlayerState.Die] = new PlayerDieState(playerController);
        }

        private PlayerStateBase[] states = new PlayerStateBase[4];

        // 상태 전환
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

        // 이전 상태로 전환
        public void UndoState()
        {
            if (prevState != null)
            {
                _ChangeState(prevState);
            }
        }

        public void Updated()
        {
            currentState?.UpdateState();
        }

        PlayerStateBase currentState;
        PlayerStateBase prevState;

        public PlayerStateBase CurrentState => currentState;
        public PlayerStateBase PrevState => prevState;
    }
}