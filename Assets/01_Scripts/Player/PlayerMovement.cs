using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Fusion;

[RequireComponent(typeof(NetworkCharacterController))]
public class PlayerMovement : NetworkBehaviour, IPlayerComponent, IPlayerAction
{
    [SerializeField] protected bool isOfflineMode = false;
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotateSpeed = 10f;
    [SerializeField] protected float arrivalThreshold = 0.1f;

    protected Vector3? currentTargetPosition;

    /// <summary>
    /// 플레이어가 이동 중인지 확인하는 변수
    /// </summary>
    bool isMoving = false;

    public bool IsMoving => isMoving;

    private NetworkCharacterController _cc;

    public bool IsActionInProgress => isActionInProgress;

    public bool CanExecuteAction => Controllable;

    public event Action OnActionCompleted;

    public void MoveForDeltaTime(Vector3 targetPosition)
    {
        targetPosition.y = 0.0f;
        Vector3 direction = (targetPosition - transform.position).normalized;
        _cc.Move(direction * moveSpeed * Runner.DeltaTime);
    }

    public void RotateForDeltaTime(Vector3 direction)
    {
        if (!HasStateAuthority) return;
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotateSpeed * Runner.DeltaTime); 
        transform.rotation = targetRotation;
    }

    public void StartMoveToNewTarget(Vector3 targetPosition, bool rotateTowardTarget = true)
    {
        StopMove();

        targetPosition.y = 0.0f;
        currentTargetPosition = targetPosition;

        isMoving = true;
    }

    private void MoveComplete()
    {
        _cc.Teleport(currentTargetPosition);
        currentTargetPosition = null;
        StopMove();

        OnActionCompleted.Invoke();
    }

    public virtual void StopMove()
    {
        isMoving = false;
    }
    private IPlayerContext context;
    private bool isActionInProgress;

    public void Initialize(IPlayerContext context, bool isOfflineMode)
    {
        this.context = context;
        this.isOfflineMode = isOfflineMode;
    }

    public void Updated()
    {
        if (isMoving)
        {
            if (Vector3.Distance(_cc.transform.position, currentTargetPosition.Value) > arrivalThreshold)
                MoveForDeltaTime(currentTargetPosition.Value);
            else
            {
                MoveComplete();
            }
        }
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
                break;
            case "F":                
                StopMove();
                break;
        }
    }

    public bool Controllable { get; set; } = true;


    public void ExecuteAction()
    {
        if (isActionInProgress)
        {
            StopMove();
        }
        isActionInProgress = true;
        StartMoveToNewTarget(context.MousePositionGetter.ClickPoint.Value);
    }

    public void StopAction()
    {
        StopMove();
    }
}
