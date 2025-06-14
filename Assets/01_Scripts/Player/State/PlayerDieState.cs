using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mundo_dodgeball.Player.StateMachine
{
    public class PlayerDieState : PlayerStateBase
    {
        public PlayerDieState(IPlayerContext playerContext) : base(playerContext)
        {
            base.playerContext = playerContext;
        }

        public override void EnterState(StateTransitionInputData inputData)
        {
            playerContext.Anim.SetTrigger("Die");
        }

        public override void ExitState()
        {
        }

        public override void NetworkUpdateState(float runnerDeltaTime)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
        }
    }
}