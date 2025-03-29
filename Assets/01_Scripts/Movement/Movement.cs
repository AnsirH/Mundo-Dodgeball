using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IMovable
{
    /// <summary> targetPosition로 deltaTime동안 이동 </summary>
    public void MoveForDeltaTime(Vector3 targetPosition, float moveSped);

    /// <summary> targetPosition로 deltaTime동안 회전 </summary>
    public void RotateForDeltaTime(Vector3 targetPosition);
}

public class Movement : MonoBehaviourPun, IMovable
{
    #region interface method
    // 델타 타임 기준 이동
    public void MoveForDeltaTime(Vector3 targetPosition, float moveSped)
    {
        Vector3 dir = (targetPosition - transform.position).normalized;

        transform.position += dir * moveSped * Time.deltaTime;
    }

    // 델타 타임 기준 회전
    public void RotateForDeltaTime(Vector3 targetPosition)
    {
        // 목표 회전값 구하기
        targetPosition.y = transform.position.y;
        Quaternion targetRotation = Quaternion.LookRotation((targetPosition - transform.position).normalized);

        // 회전
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
    #endregion

    #region public method
    /// <summary> targetPosition까지 도달할 때까지 이동 </summary>
    public void StartMoveToNewTarget(Vector3 targetPosition, bool rotateTowardTarget=true)
    {
        // 이동 초기화
        StopMove();
        // 현재 목표 위치 설정
        currentTargetPosition = targetPosition;
        // 이동 시작( MovingToTargt )
        movingCoroutine = StartCoroutine(MovingToTarget(targetPosition, rotateTowardTarget));
    }

    /// <summary> 현재 저장된 목표 위치에 도달할 때까지 이동 </summary>
    public void ContinueMoveToTarget(bool rotateTowardTarget = true)
    {
        // 저장된 위치가 없다면
        if (!currentTargetPosition.HasValue)
        {
            Debug.LogError("Playable Movement has no current target position");
            return;
        }

        // 이동 초기화
        StopMove();

        // 현재 타겟 위치로 이동 시작
        movingCoroutine = StartCoroutine(MovingToTarget(currentTargetPosition.Value, rotateTowardTarget));
    }

    /// <summary> 이동 초기화 </summary>
    public void StopMove()
    {
        // 이동 유무 확인
        // - 이동 중이면 이동 멈추고 새로운 이동 시작
        if (movingCoroutine != null)
        {
            StopCoroutine(movingCoroutine);
            movingCoroutine = null;
        }
    }
    #endregion

    #region private method
    /// <summary> 목표 위치로 이동 </summary>
    private IEnumerator MovingToTarget(Vector3 targetPosition, bool rotateTowardTarget)
    {
        // 목표 위치에 도달할 때까지 이동
        while (!CheckArrive(targetPosition))
        {
            MoveForDeltaTime(targetPosition, moveSpeed);
            if (rotateTowardTarget) RotateForDeltaTime(targetPosition);
            yield return null;
        }
    }

    /// <summary> targetPosition에 도달했는지 확인 </summary>
    private bool CheckArrive(Vector3 targetPosition)
    {
        return Vector3.Distance(transform.position, targetPosition) < 0.01f;
    }
    #endregion

    #region public variables 

    // 이동 속도
    public float moveSpeed = 1.0f;
    // 회전 속도
    public float rotateSpeed = 1080.0f;

    #endregion 

    #region private variables    
    // 이동 코루틴 저장용
    private Coroutine movingCoroutine;
    // 목표 위치 저장용
    private Vector3? currentTargetPosition;
    #endregion

    #region properties
    // 이동 중인지 확인
    public bool IsMoving { get { return movingCoroutine != null; } }
    #endregion
}