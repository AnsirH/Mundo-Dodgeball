using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerCharacterControl.State
{
    public class PlayerStateMachine
    {
        public PlayerStateMachine(IPlayerContext playerContext, IPlayerAction attack, IPlayerAction movement)
        {
            states[(int)EPlayerState.Idle] = new PlayerIdleState(playerContext);
            states[(int)EPlayerState.Move] = new PlayerMoveState(playerContext, movement);
            states[(int)EPlayerState.Attack] = new PlayerAttackState(playerContext, attack);
            states[(int)EPlayerState.Die] = new PlayerDieState(playerContext);


            // �ൿ �Ϸ� �� Idle ���·� ��ȯ
            attack.OnActionCompleted += () => ChangeState(EPlayerState.Idle);

            movement.OnActionCompleted += () => ChangeState(EPlayerState.Idle);

            ChangeState(EPlayerState.Idle);
        }

        private PlayerStateBase[] states = new PlayerStateBase[4];

        public void HandleInput(string stateName)
        {
            switch (stateName)
            {
                case "Click":
                    ChangeState(EPlayerState.Attack);
                    break;

                case "Move":
                    ChangeState(EPlayerState.Move);
                    break;
            }
        }

        // ���� ��ȯ
        private void ChangeState(EPlayerState newState)
        {
            if (newState == EPlayerState.None) return;

            ChangeState(states[(int)newState]);
        }

        private void ChangeState(PlayerStateBase newState)
        {
            if (currentState != null)
            {
                prevState = currentState;
                prevState.ExitState();
            }

            currentState = newState;
            currentState.EnterState();
        }

        // ���� ���·� ��ȯ
        public void UndoState()
        {
            if (prevState != null)
            {
                ChangeState(prevState);
            }
        }

        public void UpdateCurrentState()
        {
            currentState?.UpdateState();
        }

        PlayerStateBase currentState;
        PlayerStateBase prevState;

        public PlayerStateBase CurrentState => currentState;
        public PlayerStateBase PrevState => prevState;
    }
}