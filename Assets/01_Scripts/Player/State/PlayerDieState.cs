using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    public class PlayerDieState : PlayerStateBase
    {
        public PlayerDieState(PlayerController playerController) : base(playerController)
        {
        }

        public override void EnterState()
        {
            Debug.Log($"{GetType().Name} ���� ����");
        }

        public override void ExitState()
        {
            Debug.Log($"{GetType().Name} ���� ����");
        }

        public override void UpdateState()
        {
            //Debug.Log($"{GetType().Name} ���� ���� ��");
        }
    }
}