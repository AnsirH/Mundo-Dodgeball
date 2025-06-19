using Mundo_dodgeball.Player.StateMachine;
using System;
using UnityEngine;
using Fusion;

/// <summary>
/// ?Œë ˆ?´ì–´ ì»´í¬?ŒíŠ¸?¤ì´ ?„ìš”ë¡??˜ëŠ” ê¸°ëŠ¥???œê³µ?˜ëŠ” ?¸í„°?˜ì´??
/// </summary>
public interface IPlayerContext
{
    NetworkRunner Runner { get; }

    void ChangeState(EPlayerState state, StateTransitionInputData inputData = new());
    /// <summary> ?Œë ˆ?´ì–´???„ì¬ ?íƒœ </summary>
    PlayerStateBase CurrentState { get; }

    /// <summary> ?Œë ˆ?´ì–´??? ë‹ˆë©”ì´?°ë? ë°˜í™˜ </summary>
    Animator Anim { get; }

    /// <summary> ?Œë ˆ?´ì–´??AudioSourceë¥?ë°˜í™˜ </summary>
    PlayerSound Sound { get; }

    /// <summary> ?Œë ˆ?´ì–´???¥ë ¥ì¹?ì»´í¬?ŒíŠ¸ë¥?ë°˜í™˜ </summary>
    PlayerStats Stats { get; }

    PlayerMovement Movement { get; }

    PlayerAttack Attack { get; }

    PlayerHealth Health { get; }

    PlayerSpellActuator Spell { get; }
}