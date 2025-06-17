using Mundo_dodgeball.Player.StateMachine;
using System;
using UnityEngine;
using MyGame.Utils;
using Fusion;

/// <summary>
/// 플레이어 컴포넌트들이 필요로 하는 기능을 제공하는 인터페이스
/// </summary>
public interface IPlayerContext
{
    NetworkRunner Runner { get; }

    void ChangeState(EPlayerState state, StateTransitionInputData inputData = new());
    /// <summary> 플레이어의 현재 상태 </summary>
    PlayerStateBase CurrentState { get; }

    /// <summary> 플레이어의 애니메이터를 반환 </summary>
    Animator Anim { get; }

    /// <summary> 플레이어의 AudioSource를 반환 </summary>
    AudioSource Audio { get; }

    /// <summary> 플레이어의 능력치 컴포넌트를 반환 </summary>
    PlayerStats Stats { get; }

    PlayerMovement Movement { get; }

    PlayerAttack Attack { get; }

    PlayerHealth Health { get; }

    PlayerSpellActuator Spell { get; }
}

/// <summary>
/// 플레이어 관련 컴포넌트들이 구현해야 하는 인터페이스
/// </summary>
public interface IPlayerComponent
{
    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    void Initialize(IPlayerContext context, bool isOfflineMode);

    /// <summary>
    /// 컴포넌트가 활성화될 때 호출
    /// </summary>
    void OnEnabled();

    /// <summary>
    /// 컴포넌트가 비활성화될 때 호출
    /// </summary>
    void OnDisabled();

    public void HandleInput(NetworkInputData data);

    public bool Controllable { get; set; }
}

interface IUpdatedPlayerComponent
{
    /// <summary>
    /// 매 프레임 호출되는 업데이트
    /// </summary>
    void Updated();

    /// <summary>
    /// NetworkBehaviou 전용 Update 메서드
    /// </summary>
    void NetworkUpdated(float runnerDeltaTime);
}

// 행동 완료를 알리는 인터페이스
public interface IPlayerAction
{
    event Action OnActionCompleted;
    void ExecuteAction();
    void StopAction();
    bool IsActionInProgress { get; }
    bool CanExecuteAction { get; }
}