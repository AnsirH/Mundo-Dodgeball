using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.State
{
    public class PlayerIdleState : PlayerStateBase
    {
        public PlayerIdleState(PlayerController playerController) : base(playerController)
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