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

    // �÷��̾� ���� ���̽�
    public abstract class PlayerStateBase
    {
        public PlayerStateBase(IPlayerContext playerContext)
        {
            this.playerContext = playerContext;
        }

        protected IPlayerContext playerContext;

        // ���� ���� �� ȣ��
        public abstract void EnterState(StateTransitionInputData inputData = new());

        // ���� ���� �� ȣ��
        public abstract void ExitState();

        // ���� ������Ʈ. PlayerController Update���� ȣ��
        public abstract void UpdateState();

        // ���� ������Ʈ. PlayerController NetworkFixedUpdate���� ȣ��
        public abstract void NetworkUpdateState(float runnerDeltaTime);
    }
}