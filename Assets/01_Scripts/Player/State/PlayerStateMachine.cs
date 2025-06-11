using PlayerCharacterControl.State;
using UnityEngine;
namespace Mundo_dodgeball.Player.StateMachine
{
    /// <summary> 캐릭터의 상태를 나타낸다 </summary>
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
            // 플레이어 상태 객체 생성 및 지정
            states[(int)EPlayerState.Idle] = new PlayerIdleState(playerContext);
            states[(int)EPlayerState.Move] = new PlayerMoveState(playerContext);
            states[(int)EPlayerState.Attack] = new PlayerAttackState(playerContext);
            states[(int)EPlayerState.Die] = new PlayerDieState(playerContext);
            
            globalState = new PlayerGlobalState(playerContext);
            //// 행동형 컴포넌트 행동 종료 이벤트 등록
            //attack.OnActionCompleted += () => ChangeState(EPlayerState.Idle);

            //movement.OnActionCompleted += () => ChangeState(EPlayerState.Idle);

            // 현재 상태 Idle 상태로 초기화
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