using PlayerCharacterControl.State;
using System;
using UnityEngine;
using MyGame.Utils;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 컴포넌트들이 필요로 하는 기능을 제공하는 인터페이스
/// </summary>
public interface IPlayerContext
{
    /// <summary>
    /// 플레이어가 사망했을 때 호출
    /// </summary>
    void OnPlayerDeath();

    void InitGround(int sectionNum);

    #region properties
    /// <summary> 플레이어의 현재 상태 </summary>
    PlayerStateBase PlayerState { get; }

    /// <summary> 플레이어의 트랜스폼을 반환 </summary>
    Transform Trf { get; }

    /// <summary> 플레이어의 애니메이터를 반환 </summary>
    Animator Anim { get; }

    /// <summary> 플레이어의 AudioSource를 반환 </summary>
    AudioSource Audio { get; }

    /// <summary> 플레이어의 능력치 컴포넌트를 반환 </summary>
    PlayerStats Stats { get; }

    IMousePositionGetter MousePositionGetter { get; }

    int GroundSectionNum { get; }

    #endregion
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
    /// 매 프레임 호출되는 업데이트
    /// </summary>
    void Updated();

    /// <summary>
    /// 컴포넌트가 활성화될 때 호출
    /// </summary>
    void OnEnabled();

    /// <summary>
    /// 컴포넌트가 비활성화될 때 호출
    /// </summary>
    void OnDisabled();

    public void HandleInput(InputAction.CallbackContext context);

    public bool Controllable { get; set; }
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