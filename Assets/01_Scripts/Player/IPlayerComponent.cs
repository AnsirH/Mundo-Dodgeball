using Mundo_dodgeball.Player.StateMachine;
using System;
using UnityEngine;
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
    PlayerSound Sound { get; }

    /// <summary> 플레이어의 능력치 컴포넌트를 반환 </summary>
    PlayerStats Stats { get; }

    PlayerMovement Movement { get; }

    PlayerAttack Attack { get; }

    PlayerHealth Health { get; }

    PlayerSpellActuator Spell { get; }
}