using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public interface IMovable
{
    /// <summary> targetPosition deltaTime ̵ </summary>
    public void MoveForDeltaTime(Vector3 targetPosition, float deltaTime);

    /// <summary> targetPosition deltaTime ȸ </summary>
    public void RotateForDeltaTime(Vector3 targetPosition, float deltaTime);

    /// <summary> targetPosition   ̵ </summary>
    public void StartMoveToNewTarget(Vector3 targetPosition, bool rotateTowardTarget = true);

    /// <summary> ̵ ʱȭ </summary>
    public void StopMove();

    /// <summary> ̵  Ȯ </summary>
    public bool IsMoving { get; }

    MovementState CurrentState { get; }
    event Action<MovementState> OnStateChanged;
}

public enum MovementState
{
    Idle,
    Moving,
    Rotating,
    Stopping
}

public class Movement : MonoBehaviourPun, IMovable
{
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotateSpeed = 10f;
    [SerializeField] protected float arrivalThreshold = 0.1f;

    protected Vector3? currentTargetPosition;
    protected Coroutine moveCoroutine;
    protected Tweener moveTween;
    protected Tweener rotateTween;
    protected MovementState currentState = MovementState.Idle;

    public MovementState CurrentState => currentState;
    public event Action<MovementState> OnStateChanged;

    protected void SetState(MovementState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        OnStateChanged?.Invoke(newState);
    }

    public virtual void MoveForDeltaTime(Vector3 targetPosition, float deltaTime)
    {
        if (!photonView.IsMine) return;

        Vector3 dir = (targetPosition - transform.position).normalized;

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * deltaTime;
    }

    public virtual void RotateForDeltaTime(Vector3 targetPosition, float deltaTime)
    {
        if (!photonView.IsMine) return;

        // ǥ ȸ ϱ
        targetPosition.y = transform.position.y;

        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * deltaTime);
        }
    }

    public virtual void StartMoveToNewTarget(Vector3 targetPosition, bool rotateTowardTarget = true)
    {
        if (!photonView.IsMine) return;

        // ̵ ʱȭ
        StopMove();
        targetPosition.y = transform.position.y;
        currentTargetPosition = targetPosition;

        // 이동 거리 계산
        float distance = Vector3.Distance(transform.position, targetPosition);
        // 이동 시간 계산 (거리에 비례)
        float duration = distance / moveSpeed;

        SetState(MovementState.Moving);

        // 이동
        moveTween = transform.DOMove(targetPosition, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => OnMoveComplete());

        // 회전
        if (rotateTowardTarget)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                SetState(MovementState.Rotating);
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                rotateTween = transform.DORotateQuaternion(targetRotation, 0.2f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => {
                        if (currentState == MovementState.Rotating)
                            SetState(MovementState.Moving);
                    });
            }
        }
    }

    protected virtual void OnMoveComplete()
    {
        moveTween = null;
        rotateTween = null;
        currentTargetPosition = null;
        SetState(MovementState.Idle);
    }

    public virtual void StopMove()
    {
        if (currentState == MovementState.Idle) return;
        
        SetState(MovementState.Stopping);
        moveTween?.Kill();
        rotateTween?.Kill();
        moveTween = null;
        rotateTween = null;
        OnMoveComplete();
    }

    public bool IsMoving => currentState == MovementState.Moving || currentState == MovementState.Rotating;

    protected virtual void OnDestroy()
    {
        StopMove();
    }
}