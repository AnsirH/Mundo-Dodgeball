using PlayerCharacterControl.State;
using UnityEngine;
namespace Mundo_dodgeball.Player.StateMachine
{
    /// <summary> ìºë¦­?°ì˜ ?íƒœë¥??˜í??¸ë‹¤ </summary>
    public enum EPlayerState
    {
        None = -1,
        Idle = 0,
        Move,
        Attack,
        Die
    }

    public class PlayerStateMachine
    {
        public PlayerStateMachine(IPlayerContext playerContext)
        {
            // ?Œë ˆ?´ì–´ ?íƒœ ê°ì²´ ?ì„± ë°?ì§€??
            states[(int)EPlayerState.Idle] = new PlayerIdleState(playerContext);
            states[(int)EPlayerState.Move] = new PlayerMoveState(playerContext);
            states[(int)EPlayerState.Attack] = new PlayerAttackState(playerContext);
            states[(int)EPlayerState.Die] = new PlayerDieState(playerContext);
            
            globalState = new PlayerGlobalState(playerContext);
            //// ?‰ë™??ì»´í¬?ŒíŠ¸ ?‰ë™ ì¢…ë£Œ ?´ë²¤???±ë¡
            //attack.OnActionCompleted += () => ChangeState(EPlayerState.Idle);

            //movement.OnActionCompleted += () => ChangeState(EPlayerState.Idle);

            // ?„ì¬ ?íƒœ Idle ?íƒœë¡?ì´ˆê¸°??
            ChangeState(EPlayerState.Idle);
        }

        public void ChangeState(EPlayerState newState)
        {
            if (newState == EPlayerState.None) return;

            _ChangeState(states[(int)newState]);
        }
        
        public void ChangeState(EPlayerState newState, StateTransitionInputData inputData)
        {
            if (newState == EPlayerState.None) return;

            _ChangeState(states[(int)newState], inputData);
        }

        private void _ChangeState(PlayerStateBase newState, StateTransitionInputData inputData = new())
        {
            if (currentState != null && currentState != newState)
            {
                prevState = currentState;
                prevState.ExitState();
            }

            currentState = newState;
            currentState.EnterState(inputData);
        }

        public void RevertToPreviousState()
        {
            if (prevState != null)
            {
                _ChangeState(prevState);
            }
        }

        public void Updated()
        {
            currentState?.UpdateState();
            globalState?.UpdateState();
        }

        public void NetworkUpdated(float runnerDeltaTime)
        {
            currentState?.NetworkUpdateState(runnerDeltaTime);
            globalState?.NetworkUpdateState(runnerDeltaTime);
        }

        private PlayerStateBase[] states = new PlayerStateBase[4];

        PlayerStateBase currentState;
        PlayerStateBase globalState;
        PlayerStateBase prevState;

        public PlayerStateBase CurrentState => currentState;
        public PlayerStateBase PrevState => prevState;
    }
}