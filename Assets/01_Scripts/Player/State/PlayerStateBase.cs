using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerCharacterControl.State
{
    // �÷��̾� ���� ���̽�
    public abstract class PlayerStateBase
    {
        public PlayerStateBase(IPlayerContext playerContext)
        {
            this.context = playerContext;
        }

        protected IPlayerContext context;

        // ���� ���� �� ȣ��
        public abstract void EnterState();

        // ���� ���� �� ȣ��
        public abstract void ExitState();

        // ���� ������Ʈ. PlayerController Update���� ȣ��
        public abstract void UpdateState();
    }
}