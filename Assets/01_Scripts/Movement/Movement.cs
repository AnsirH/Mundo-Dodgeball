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
}

public class Movement : MonoBehaviourPun, IMovable
{
    [SerializeField] protected bool isOfflineMode = false;
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotateSpeed = 10f;
    [SerializeField] protected float arrivalThreshold = 0.1f;

    protected Vector3? currentTargetPosition;
    protected Coroutine moveCoroutine;
    protected Tweener moveTween;
    protected Tweener rotateTween;


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

    [PunRPC]
    public void RPC_StartMove(Vector3 targetPosition)
    {
        StartMoveToNewTargetInternal(targetPosition);
    }

    private void StartMoveToNewTargetInternal(Vector3 targetPosition, bool rotateTowardTarget = true)
    {
        StopMove();
        targetPosition.y = transform.position.y;
        currentTargetPosition = targetPosition;

        // 이동 거리 계산
        float distance = Vector3.Distance(transform.position, targetPosition);
        // 이동 시간 계산 (거리에 비례)
        float duration = distance / moveSpeed;


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
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                rotateTween = transform.DORotateQuaternion(targetRotation, 0.2f)
                    .SetEase(Ease.OutQuad).OnComplete(() => rotateTween = null);
            }
        }
    }

    public virtual void StartMoveToNewTarget(Vector3 targetPosition, bool rotateTowardTarget = true)
    {
        if (isOfflineMode)
        {
            // 오프라인 모드: 직접 이동
            StartMoveToNewTargetInternal(targetPosition);
        }
        else if (photonView.IsMine)
        {
            // 온라인 모드: RPC로 이동 요청
            photonView.RPC("RPC_StartMove", RpcTarget.All, targetPosition);
        }
    }

    protected virtual void OnMoveComplete()
    {
        moveTween = null;
        currentTargetPosition = null;
    }

    public virtual void StopMove()
    {
        moveTween?.Kill();
        rotateTween?.Kill();
        moveTween = null;
        rotateTween = null;
    }

    public bool IsMoving => moveTween != null;

    protected virtual void OnDestroy()
    {
        StopMove();
    }
}