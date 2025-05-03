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
        StopMove();
    }

    public void OnEnabled()
    {
        //throw new System.NotImplementedException();
    }

    //private Vector3 GetAdjustedTargetPosition(Vector3 targetPosition)
    //{

    //}
}
