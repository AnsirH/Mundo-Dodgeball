using DG.Tweening.Core.Easing;
using UnityEngine;

namespace Mundo_dodgeball.Player.StateMachine
{
    public class PlayerGlobalState : PlayerStateBase
    {
        public PlayerGlobalState(IPlayerContext playerContext) : base(playerContext)
        {
            this.playerContext = playerContext;
        }

        public override void EnterState(StateTransitionInputData inputData)
        {
        }

        public override void ExitState()
        {

        }

        public override void UpdateState()
        {
            //if (playerContext.CurrentState is not PlayerDieState && playerContext.Health.IsDead)
            //{
            //    playerContext.ChangeState(EPlayerState.Die);
            //}
        }

        public override void NetworkUpdateState(float runnerDeltaTime)
        {
        }
    }
}