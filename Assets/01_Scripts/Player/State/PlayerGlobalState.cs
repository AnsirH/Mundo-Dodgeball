using UnityEngine;

namespace Mundo_dodgeball.Player.StateMachine
{
    public class PlayerGlobalState : PlayerStateBase
    {
        public PlayerGlobalState(IPlayerContext playerContext) : base(playerContext)
        {
            _playerContext = playerContext;
        }

        public override void EnterState(StateTransitionInputData inputData)
        {
        }

        public override void ExitState()
        {

        }

        public override void UpdateState()
        {
        }

        public override void NetworkUpdateState(float runnerDeltaTime)
        {
        }

        private IPlayerContext _playerContext;

    }
}