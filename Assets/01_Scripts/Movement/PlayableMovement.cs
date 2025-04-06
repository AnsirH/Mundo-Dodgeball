using System.Collections;
using UnityEngine;
using System;
using Photon.Pun;

public class PlayableMovement : Movement, IPlayerComponent, IPlayerAction
{
    private IPlayerContext playerContext;
    private bool isActionInProgress;

    public void Initialize(IPlayerContext context)
    {
        playerContext = context;
    }

    public void Updated()
    {
    }

    public void ExecuteAction()
    {
        if (!playerContext.IsLocalPlayer()) return;

        Vector3? mousePos = playerContext.GetMousePosition();
        if (!mousePos.HasValue) return;

        isActionInProgress = true;
        StartMoveToNewTarget(mousePos.Value);
    }

    public bool IsActionInProgress => isActionInProgress;

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

