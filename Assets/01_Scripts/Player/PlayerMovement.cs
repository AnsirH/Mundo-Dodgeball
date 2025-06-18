using UnityEngine;
using System;
using Fusion;
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float arrivalThreshold = 0.1f;

    [Networked] private Vector3 currentTargetPosition { get; set; }
    [Networked] private Vector3 TargetDirection { get; set; }

    public bool IsArrived { get { return Vector3.Distance(_cc.transform.position, currentTargetPosition) <= arrivalThreshold; } }

    private NetworkCharacterController _cc;


    public void Initialize(IPlayerContext context)
    {
        this.context = context;
        _cc = GetComponent<NetworkCharacterController>();
        _cc.maxSpeed = context.Stats.GetMoveSpeed();
    }

    public void Teleport(Vector3 targetPosition)
    {
        _cc.Teleport(targetPosition);
    }

    public void SetRotation(Quaternion rotation)
    {
        _cc.transform.transform.rotation = rotation;
    }

    /// <summary>
    /// 이동 지점 설정
    /// </summary>
    /// <param name="targetPosition"></param>
    public bool TrySetMovementTarget(Vector3 targetPosition)
    {
        targetPosition.y = 0.0f;
        currentTargetPosition = targetPosition;

        if (IngameController.Instance != null && IngameController.Instance.Ground != null)
        {
            if (IngameController.Instance.Ground.GetAdjustedPoint(Object.InputAuthority.PlayerId - 1, context.Movement.transform.position, currentTargetPosition, out Vector3 adjustedPoint))
                currentTargetPosition = adjustedPoint;
            else
            {
                currentTargetPosition = default;
            }
        }

        if (currentTargetPosition == default) return false;
        else if (IsArrived)
        {
            currentTargetPosition = default;
            return false;
        }
        else return true;
    }

    /// <summary>
    /// 이동 정지. 현재 위치로 캐릭터 위치 지정
    /// </summary>
    public void StopMove()
    {
        currentTargetPosition = default;
    }

    /// <summary>
    /// 이동 완료. 목적지로 캐릭터 위치 지정
    /// </summary>
    public void CompleteMove()
    {
        _cc.Teleport(currentTargetPosition);
        currentTargetPosition = default;
    }

    public void RotateForDeltaTime(Quaternion currentRotation, Vector3 direction, float rotateSpeed)
    {
        if (!Object.HasStateAuthority) return;
        
        TargetDirection = direction;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        _cc.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotateSpeed * Runner.DeltaTime);
    }

    //--- PRIVATE METHOD ---
    private void MoveForDeltaTime(Vector3 normalizedDirection)
    {
        if (IsArrived)
            return;
        _cc.Move(/*context.Stats.GetMoveSpeed() * Runner.DeltaTime * */normalizedDirection);
    }

    private void MoveTowardTarget()
    {
        if (currentTargetPosition == default)
        {
            return;
        }
        Vector3 normalizedDirection = (currentTargetPosition - _cc.transform.position).normalized;
        MoveForDeltaTime(normalizedDirection);
    }

    private IPlayerContext context;

    public override void FixedUpdateNetwork()
    {
        if (currentTargetPosition != default)
        {
            MoveTowardTarget();
        }
    }
}
