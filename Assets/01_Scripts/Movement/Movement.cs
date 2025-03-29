using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IMovable
{
    /// <summary> ��ǥ ��ġ�� deltaTime���� �̵� </summary>
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
        // �̵� �ʱ�ȭ
        StopMove();
        // ���� ��ǥ ��ġ ����
        currentTargetPosition = targetPosition;
        // �̵� ����( MovingToTargt )
        movingCoroutine = StartCoroutine(MovingToTarget(targetPosition));
    }

    /// <summary> ���� ����� ��ǥ ��ġ�� �̵��ϱ� </summary>
    public void ContinueMoveToTarget()
    {
        // ����� ��ġ�� ���ٸ�
        if (!currentTargetPosition.HasValue)
            return;

        // �̵� �ʱ�ȭ
        StopMove();

        // ���� Ÿ�� ��ġ�� �̵� ����
        movingCoroutine = StartCoroutine(MovingToTarget(currentTargetPosition.Value));
    }

    /// <summary> �̵� �ʱ�ȭ </summary>
    public void StopMove()
    {
        // �̵� ���� Ȯ��
        // - �̵� ���̸� �̵� ���߰� ���ο� �̵� ����
        if (movingCoroutine != null)
        {
            StopCoroutine(movingCoroutine);
            movingCoroutine = null;
        }
    }
    #endregion

    #region private method

    /// <summary> ��ǥ ��ġ�� �̵� </summary>
    private IEnumerator MovingToTarget(Vector3 targetPosition)
    {
        // ��ǥ ��ġ�� ������ ������ �̵�
        while (!CheckArrive(targetPosition))
        {
            MoveForDeltaTime(targetPosition, moveSpeed);
            yield return null;
        }
    }

    /// <summary> targetPosition�� �����ߴ��� Ȯ�� </summary>
    /// <returns>���� ������ true</returns>
    private bool CheckArrive(Vector3 targetPosition)
    {
        return Vector3.Distance(transform.position, targetPosition) < 0.01f;
    }
    #endregion

    #region variables
    // ��ġ
    private float moveSpeed = 1.0f;
    // �̵� �ڷ�ƾ �����
    private Coroutine movingCoroutine;
    // ��ǥ ��ġ �����
    private Vector3? currentTargetPosition;
    #endregion

    #region properties
    // �̵� ������ Ȯ��
    public bool IsMoving { get { return movingCoroutine != null; } }
    #endregion
}