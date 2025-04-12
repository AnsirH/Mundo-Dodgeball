using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviourPunCallbacks, IPlayerComponent, IPlayerAction
{
    private IPlayerContext context;

    public float attackPower = 80.0f;
    private float attackDuration = 0.75f;

    private Coroutine currentAttackRoutine;

    [Header("References")]
    // ���� �߻�ü
    [SerializeField] private AxeShooter axeShooter;

    // ĳ���� ���� �𵨸�
    [SerializeField] private GameObject axeObj;

    #region IPlayerAction Implementation
    public event Action OnActionCompleted;

    public bool IsActionInProgress { get; private set; } = false;

    public bool CanExecuteAction => axeShooter.CanShoot && axeShooter.IsRangeActive;

    public bool Controllable { get; set; } = true;

    public void ExecuteAction()
    {
        if (IsActionInProgress || !axeShooter.IsRangeActive) return;

        ActivateRange(false);
        Vector3? targetPoint = context.GetMousePosition();
        if (targetPoint.Value == null) return;
        Vector3 direction = (targetPoint.Value - context.Pos).normalized;
        direction.y = 0.0f;
        transform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.25f).onComplete += () => { axeShooter.SpawnProjectile(); };

        IsActionInProgress = true;
        currentAttackRoutine = StartCoroutine(AttackRoutine());

    }
    #endregion

    #region IPlayerComponent Implementation
    public void Initialize(IPlayerContext context)
    {
        this.context = context;
    }

    public void Updated()
    {
    }

    public void OnEnabled()
    {
        //throw new System.NotImplementedException();
    }

    public void OnDisabled()
    {
        CancelAttack();

        if (currentAttackRoutine != null)
        {
            StopCoroutine(currentAttackRoutine);
            IsActionInProgress = false;
        }
    }
    #endregion


    // ���� �ڷ�ƾ. �ð� ������ �Ϸ� �̺�Ʈ ����
    private IEnumerator AttackRoutine()
    {
        // ���� �ִϸ��̼� �� ����
        yield return new WaitForSeconds(attackDuration);

        IsActionInProgress = false;
        axeObj.SetActive(true);
        OnActionCompleted?.Invoke();
    }

    // ���� ���� Ȱ��ȭ �޼���
    public void ActivateRange(bool active)
    {
        axeShooter.ActivateRange(active);
    }

    public void CancelAttack()
    {
        ActivateRange(false);

        // ȸ�� ����
        transform.DOKill();
    }
}