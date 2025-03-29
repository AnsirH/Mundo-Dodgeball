using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IMovable
{
    /// <summary> targetPosition�� deltaTime���� �̵� </summary>
    public void MoveForDeltaTime(Vector3 targetPosition, float moveSped);

    /// <summary> targetPosition�� deltaTime���� ȸ�� </summary>
    public void RotateForDeltaTime(Vector3 targetPosition);
}

public class Movement : MonoBehaviourPun, IMovable
{
    #region interface method
    // ��Ÿ Ÿ�� ���� �̵�
    public void MoveForDeltaTime(Vector3 targetPosition, float moveSped)
    {
        Vector3 dir = (targetPosition - transform.position).normalized;

        transform.position += dir * moveSped * Time.deltaTime;
    }

    // ��Ÿ Ÿ�� ���� ȸ��
    public void RotateForDeltaTime(Vector3 targetPosition)
    {
        // ��ǥ ȸ���� ���ϱ�
        targetPosition.y = transform.position.y;
        Quaternion targetRotation = Quaternion.LookRotation((targetPosition - transform.position).normalized);

        // ȸ��
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }
    #endregion

    #region public method
    /// <summary> targetPosition���� ������ ������ �̵� </summary>
    public void StartMoveToNewTarget(Vector3 targetPosition, bool rotateTowardTarget=true)
    {
        // �̵� �ʱ�ȭ
        StopMove();
        // ���� ��ǥ ��ġ ����
        currentTargetPosition = targetPosition;
        // �̵� ����( MovingToTargt )
        movingCoroutine = StartCoroutine(MovingToTarget(targetPosition, rotateTowardTarget));
    }

    /// <summary> ���� ����� ��ǥ ��ġ�� ������ ������ �̵� </summary>
    public void ContinueMoveToTarget(bool rotateTowardTarget = true)
    {
        // ����� ��ġ�� ���ٸ�
        if (!currentTargetPosition.HasValue)
        {
            Debug.LogError("Playable Movement has no current target position");
            return;
        }

        // �̵� �ʱ�ȭ
        StopMove();

        // ���� Ÿ�� ��ġ�� �̵� ����
        movingCoroutine = StartCoroutine(MovingToTarget(currentTargetPosition.Value, rotateTowardTarget));
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
    private IEnumerator MovingToTarget(Vector3 targetPosition, bool rotateTowardTarget)
    {
        // ��ǥ ��ġ�� ������ ������ �̵�
        while (!CheckArrive(targetPosition))
        {
            MoveForDeltaTime(targetPosition, moveSpeed);
            if (rotateTowardTarget) RotateForDeltaTime(targetPosition);
            yield return null;
        }
    }

    /// <summary> targetPosition�� �����ߴ��� Ȯ�� </summary>
    private bool CheckArrive(Vector3 targetPosition)
    {
        return Vector3.Distance(transform.position, targetPosition) < 0.01f;
    }
    #endregion

    #region public variables 

    // �̵� �ӵ�
    public float moveSpeed = 1.0f;
    // ȸ�� �ӵ�
    public float rotateSpeed = 1080.0f;

    #endregion 

    #region private variables    
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