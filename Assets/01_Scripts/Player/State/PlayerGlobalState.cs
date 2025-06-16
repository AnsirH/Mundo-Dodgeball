using DG.Tweening.Core.Easing;
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
            if (!_playerContext.Health.IsDead)
                //_playerContext.Stats.HandleHealthRegen(runnerDeltaTime);

            if (!isDie && _playerContext.CurrentStatData.Health <= 0.0f)
            {
                isDie = true;
                _playerContext.ChangeState(EPlayerState.Die);
            }
            Debug.Log(_playerContext.CurrentStatData.Health);
        }

        private bool isDie = false;
        private IPlayerContext _playerContext;
    }
}