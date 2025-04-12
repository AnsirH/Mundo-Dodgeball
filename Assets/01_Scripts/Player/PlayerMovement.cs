using System.Collections;
using UnityEngine;
using System;
using Photon.Pun;

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

    public void ExecuteAction()
    {
        if (!context.IsLocalPlayer()) return;


        Vector3? mousePos = context.GetMousePosition();
        if (!mousePos.HasValue) return;

        if (isActionInProgress)
        {
            StopMove();
        }
        isActionInProgress = true;
        StartMoveToNewTarget(mousePos.Value);
    }

    public bool IsActionInProgress => isActionInProgress;

    public bool CanExecuteAction => Controllable;

    public bool Controllable { get; set; } = true;

    public event Action OnActionCompleted;

    protected override void OnMoveComplete()
    {
        base.OnMoveComplete();
        isActionInProgress = false;
        OnActionCompleted?.Invoke();
    }

    public void OnDisabled()
    {
        //throw new System.NotImplementedException();
    }

    public void OnEnabled()
    {
        //throw new System.NotImplementedException();
    }
}

