using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    public class PlayerAttackState : PlayerStateBase
    {
        public PlayerAttackState(IPlayerContext playerContext, IPlayerAction attack) : base(playerContext)
        {
            this.attack = attack;
        }

        public override void EnterState()
        {
            attack.ExecuteAction();
        }
        public override void ExitState()
        {
        }

        public override void UpdateState()
        {
        }

        private IPlayerAction attack;

    }
}