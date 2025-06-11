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
            if (_playerContext.CurrentState is not PlayerAttackState && 
                _playerContext.CurrentState is not PlayerDieState)
            {

            }
        }

        public override void NetworkUpdateState(float runnerDeltaTime)
        {
        }

        private IPlayerContext _playerContext;

    }
}