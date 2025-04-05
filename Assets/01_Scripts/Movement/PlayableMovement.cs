using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MyGame.Utils;
using System;

public class PlayableMovement : Movement, IPlayerComponent, IPlayerAction
{
    public bool IsActionInProgress { get; private set; } = false;

    public event Action OnActionCompleted;

    public void ExecuteAction()
    {
        Vector3? mousePoint = Utility.GetMousePosition(Camera.main);
        if (mousePoint.HasValue)
        {
            StartMoveToNewTarget(mousePoint.Value);
            IsActionInProgress = true;
        }
    }

    public void Initialize(IPlayerContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnDisabled()
    {
        //throw new System.NotImplementedException();
    }

    public void OnEnabled()
    {
        //throw new System.NotImplementedException();
    }

    public void Updated()
    {
        if (IsActionInProgress)
        {
            if (!IsMoving)
            {
                IsActionInProgress = false;
                OnActionCompleted?.Invoke();
            }
        }
    }
}
