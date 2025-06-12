using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float arrivalThreshold = 0.1f;

    private Vector3 currentTargetPosition = Vector3.zero;

    public bool IsArrived { get { return Vector3.Distance(_cc.transform.position, currentTargetPosition) <= arrivalThreshold || currentTargetPosition == Vector3.zero; } }

    private NetworkCharacterController _cc;

    private Ground ground;

    public void SetGround(Ground ground) { this.ground = ground; }

    //public bool IsActionInProgress => isActionInProgress;

    //public bool CanExecuteAction => Controllable;

    //public event Action OnActionCompleted;



    public void Initialize(IPlayerContext context)
    {
        this.context = context;
        _cc = GetComponent<NetworkCharacterController>();
    }

    public void Teleport(Vector3 targetPosition)
    {
        _cc.Teleport(targetPosition);
    }

    public void MoveTowardTarget()
    {
        if (currentTargetPosition == Vector3.zero)
        {
            return;
        }
        Vector3 normalizedDirection = (currentTargetPosition - _cc.transform.position).normalized;
        MoveForDeltaTime(normalizedDirection, Runner.DeltaTime);
    }

    /// <summary>
    /// 이동 지점 설정
    /// </summary>
    /// <param name="targetPosition"></param>
    public void SetMovementTarget(Vector3 targetPosition)
    {
        _cc.Velocity = Vector3.zero;
        targetPosition.y = 0.0f;
        currentTargetPosition = targetPosition;

        if (IngameController.Instance != null)
        {
            ground.GetAdjustedPoint(IngameController.Instance.GetPlayerIndex(context), context.Movement.transform.position, currentTargetPosition, out Vector3 adjustedPoint);
            currentTargetPosition = adjustedPoint;
        }
    }

    public void CompleteMove()
    {
        _cc.Teleport(currentTargetPosition);
        currentTargetPosition = Vector3.zero;
    }

    public void RotateForDeltaTime(Quaternion currentRotation, Vector3 direction, float runnerDeltaTime)
    {
        Quaternion targetRotation = Quaternion.Slerp(currentRotation, Quaternion.LookRotation(direction), rotateSpeed * runnerDeltaTime);
        _cc.transform.rotation = targetRotation;
    }

    private void MoveForDeltaTime(Vector3 normalizedDirection, float runnerDeltaTime)
    {
        _cc.Move(context.Stats.GetMoveSpeed() * runnerDeltaTime * normalizedDirection);
    }
    private IPlayerContext context;
}
