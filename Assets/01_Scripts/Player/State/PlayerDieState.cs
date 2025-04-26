using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl.State
{
    public class PlayerDieState : PlayerStateBase
    {
        public PlayerDieState(IPlayerContext playerContext) : base(playerContext)
        {
            context = playerContext;
        }

        public override void EnterState()
        {
            context.Anim.SetTrigger("Die");
        }

        public override void ExitState()
        {
        }

        public override void UpdateState()
        {
        }
    }
}