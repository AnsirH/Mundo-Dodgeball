using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Fusion;
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float arrivalThreshold = 0.1f;

    private Vector3 currentTargetPosition;
    [Networked] private Vector3 CurrentPosition { get; set; }
    [Networked] private Quaternion CurrentRotation { get; set; }
    [Networked] private Vector3 TargetDirection { get; set; }

    public bool IsArrived { get { return Vector3.Distance(_cc.transform.position, currentTargetPosition) <= arrivalThreshold || currentTargetPosition == Vector3.zero; } }

    private NetworkCharacterController _cc;

    public void Initialize(IPlayerContext context)
    {
        this.context = context;
        _cc = GetComponent<NetworkCharacterController>();
        CurrentPosition = _cc.transform.position;
        CurrentRotation = _cc.transform.rotation;
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
    public bool SetMovementTarget(Vector3 targetPosition)
    {
        _cc.Velocity = Vector3.zero;
        targetPosition.y = 0.0f;
        currentTargetPosition = targetPosition;

        if (IngameController.Instance != null && IngameController.Instance.Ground != null)
        {
            if (IngameController.Instance.Ground.GetAdjustedPoint(Object.InputAuthority.PlayerId - 1, context.Movement.transform.position, currentTargetPosition, out Vector3 adjustedPoint))
                currentTargetPosition = adjustedPoint;
            else
            {
                currentTargetPosition = Vector3.zero;
            }
        }

        if (currentTargetPosition == Vector3.zero) return false;
        else return true;
    }

    public void CompleteMove()
    {
        _cc.Teleport(currentTargetPosition);
        currentTargetPosition = Vector3.zero;
    }

    public void RotateForDeltaTime(Quaternion currentRotation, Vector3 direction, float rotateSpeed)
    {
        if (!Object.HasStateAuthority) return;
        
        TargetDirection = direction;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        CurrentRotation = Quaternion.Slerp(currentRotation, targetRotation, rotateSpeed * Runner.DeltaTime);
        _cc.transform.rotation = CurrentRotation;
    }

    //--- PRIVATE METHOD ---
    private void MoveForDeltaTime(Vector3 normalizedDirection, float runnerDeltaTime)
    {
        //if (!Object.HasStateAuthority) return;
        _cc.Move(context.Stats.GetMoveSpeed() * runnerDeltaTime * normalizedDirection);
        CurrentPosition = _cc.transform.position;
        CurrentRotation = _cc.transform.rotation;
    }

    private IPlayerContext context;
}
