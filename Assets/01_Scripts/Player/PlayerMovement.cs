using System.Collections;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerMovement : Movement, IPlayerComponent, IPlayerAction
{
    private IPlayerContext context;
    private bool isActionInProgress;

    public void Initialize(IPlayerContext context, bool isOfflineMode)
    {
        this.context = context;
        this.isOfflineMode = isOfflineMode;
    }

    public void Updated()
    {
    }

    public void OnDisabled()
    {
        StopMove();
    }

    public void OnEnabled()
    {
        StopMove();
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        switch (context.action.name)
        {
            case "Move":
                break;
            case "Click":
                StopMove();
                OnMoveComplete();
                break;
            case "F":                
                StopMove();
                OnMoveComplete();
                break;
        }
    }

    public bool Controllable { get; set; } = true;


    public void ExecuteAction()
    {
        if (!photonView.IsMine || !context.MousePositionGetter.ClickPoint.HasValue) return;

        if (isActionInProgress)
        {
            StopMove();
        }
        isActionInProgress = true;
        StartMoveToNewTarget(context.MousePositionGetter.ClickPoint.Value);
    }

    public bool IsActionInProgress => isActionInProgress;

    public bool CanExecuteAction => Controllable;

    public event Action OnActionCompleted;

    protected override void OnMoveComplete()
    {
        base.OnMoveComplete();
        isActionInProgress = false;
        OnActionCompleted?.Invoke();
    }

    public void StopAction()
    {
        StopMove();
    }
}
