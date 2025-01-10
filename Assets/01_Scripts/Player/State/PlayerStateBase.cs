using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    // �÷��̾� ���� ���̽�
    public abstract class PlayerStateBase
    {
        public PlayerStateBase(PlayerController playerController)
        {
            this.playerController = playerController;
        }

        protected PlayerController playerController;

        public PlayerController PlayerController => playerController;

        // ���� ���� �� ȣ��
        public abstract void EnterState();

        // ���� ���� �� ȣ��
        public abstract void ExitState();

        // ���� ������Ʈ. PlayerController Update���� ȣ��
        public abstract void UpdateState();
    }
}