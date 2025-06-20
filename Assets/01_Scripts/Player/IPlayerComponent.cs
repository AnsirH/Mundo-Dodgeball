using Mundo_dodgeball.Player.StateMachine;
using System;
using UnityEngine;
using Fusion;

/// <summary>
/// ?๋ ?ด์ด ์ปดํฌ?ํธ?ค์ด ?์๋ก??๋ ๊ธฐ๋ฅ???๊ณต?๋ ?ธํฐ?์ด??
/// </summary>
public interface IPlayerContext
{
    NetworkRunner Runner { get; }

    void ChangeState(EPlayerState state, StateTransitionInputData inputData = new());
    /// <summary> ?๋ ?ด์ด???์ฌ ?ํ </summary>
    PlayerStateBase CurrentState { get; }

    /// <summary> ?๋ ?ด์ด??? ๋๋ฉ์ด?ฐ๋? ๋ฐํ </summary>
    Animator Anim { get; }

    /// <summary> ?๋ ?ด์ด??AudioSource๋ฅ?๋ฐํ </summary>
    PlayerSound Sound { get; }

    /// <summary> ?๋ ?ด์ด???ฅ๋ ฅ์น?์ปดํฌ?ํธ๋ฅ?๋ฐํ </summary>
    PlayerStats Stats { get; }

    PlayerMovement Movement { get; }

    PlayerAttack Attack { get; }

    PlayerHealth Health { get; }

    PlayerSpellActuator Spell { get; }
}