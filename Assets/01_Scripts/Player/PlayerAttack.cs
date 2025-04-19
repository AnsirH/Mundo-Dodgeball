using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour, IPlayerComponent, IPlayerAction
{
    private IPlayerContext context;

    public float attackPower = 80.0f;
    private float attackDuration = 0.25f;

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

        PlayAttackAnimation();
        ActivateRange(false);
        Vector3? targetPoint = context.GetMousePosition();
        if (!targetPoint.HasValue) return;
        Vector3 direction = (targetPoint.Value - context.Pos).normalized;
        direction.y = 0.0f;
        transform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.25f).onComplete += () => { axeShooter.SpawnProjectile(targetPoint.Value); };

        IsActionInProgress = true;
        currentAttackRoutine = StartCoroutine(AttackRoutine());

    }
    #endregion

    #region IPlayerComponent Implementation
    public void Initialize(IPlayerContext context, bool isOfflineMode)
    {
        this.context = context;
        axeShooter.Initialize(context, isOfflineMode);
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

    // �ִϸ��̼� Ʈ���� ����
    void PlayAttackAnimation()
    {
        context.Anim.SetTrigger("Attack");
        context.p_PhotonView.RPC("RPC_PlayAnimation", RpcTarget.Others, "Attack");
    }

    [PunRPC]
    void RPC_PlayAnimation(string triggerName)
    {
        if (!context.p_PhotonView.IsMine)
            context.Anim.SetTrigger(triggerName);
    }

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
        if (!context.p_PhotonView.IsMine) { return; }
        if (active == true)
        {
            if (axeShooter.CanShoot)
                axeShooter.ActivateRange(true);
        }
        else
            axeShooter.ActivateRange(false);
    }

    public void CancelAttack()
    {
        ActivateRange(false);

        // ȸ�� ����
        transform.DOKill();
    }
}