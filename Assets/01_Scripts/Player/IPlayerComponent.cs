using Mundo_dodgeball.Player.StateMachine;
using System;
using UnityEngine;
using Fusion;

/// <summary>
/// ?�레?�어 컴포?�트?�이 ?�요�??�는 기능???�공?�는 ?�터?�이??
/// </summary>
public interface IPlayerContext
{
    NetworkRunner Runner { get; }

    void ChangeState(EPlayerState state, StateTransitionInputData inputData = new());
    /// <summary> ?�레?�어???�재 ?�태 </summary>
    PlayerStateBase CurrentState { get; }

    /// <summary> ?�레?�어???�니메이?��? 반환 </summary>
    Animator Anim { get; }

    /// <summary> ?�레?�어??AudioSource�?반환 </summary>
    PlayerSound Sound { get; }

    /// <summary> ?�레?�어???�력�?컴포?�트�?반환 </summary>
    PlayerStats Stats { get; }

    PlayerMovement Movement { get; }

    PlayerAttack Attack { get; }

    PlayerHealth Health { get; }

    PlayerSpellActuator Spell { get; }
}