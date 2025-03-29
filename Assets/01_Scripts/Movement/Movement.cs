using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IMovable
{
    /// <summary> 목표 위치로 deltaTime동안 이동 </summary>
    public void MoveForDeltaTime(Vector3 targetPosition, float moveSped);
}

public class Movement : MonoBehaviour, IMovable
{
    #region public method

    public void MoveForDeltaTime(Vector3 targetPosition, float moveSped)
    {
        Vector3 dir = (targetPosition - transform.position).normalized;

        transform.position += dir * moveSped * Time.deltaTime;
    }

    public void StartMoveToNewTarget(Vector3 targetPosition)
    {
        // 이동 초기화
        StopMove();
        // 현재 목표 위치 설정
        currentTargetPosition = targetPosition;
        // 이동 시작( MovingToTargt )
        movingCoroutine = StartCoroutine(MovingToTarget(targetPosition));
    }

    /// <summary> 현재 저장된 목표 위치로 이동하기 </summary>
    public void ContinueMoveToTarget()
    {
        // 저장된 위치가 없다면
        if (!currentTargetPosition.HasValue)
            return;

        // 이동 초기화
        StopMove();

        // 현재 타겟 위치로 이동 시작
        movingCoroutine = StartCoroutine(MovingToTarget(currentTargetPosition.Value));
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
    private IEnumerator MovingToTarget(Vector3 targetPosition)
    {
        // 목표 위치에 도달할 때까지 이동
        while (!CheckArrive(targetPosition))
        {
            MoveForDeltaTime(targetPosition, moveSpeed);
            yield return null;
        }
    }

    /// <summary> targetPosition에 도달했는지 확인 </summary>
    /// <returns>도달 했으면 true</returns>
    private bool CheckArrive(Vector3 targetPosition)
    {
        return Vector3.Distance(transform.position, targetPosition) < 0.01f;
    }
    #endregion

    #region variables
    // 위치
    private float moveSpeed = 1.0f;
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