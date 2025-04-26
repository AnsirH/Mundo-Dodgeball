using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviourPun, IPlayerComponent, IPlayerAction
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

        // ���� ���� ���÷��� ��Ȱ��ȭ
        ActivateRange(false);

        // ���� ���� ����
        currentAttackRoutine = StartCoroutine(AttackRoutine());

        // ���� ���� Ȱ��ȭ
        IsActionInProgress = true;
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
    }

    // ���� �ڷ�ƾ. �ð� ������ �Ϸ� �̺�Ʈ ����
    private IEnumerator AttackRoutine()
    {
        // ���콺 ��ġ Ȯ��
        if (!context.MousePositionGetter.ClickPoint.HasValue) yield break;

        // ���� ���� ���
        Vector3 direction = (context.MousePositionGetter.ClickPoint.Value - context.Pos).normalized;
        direction.y = 0.0f;

        // ���� �ִϸ��̼�
        PlayAttackAnimation();

        // ���� �������� ȸ��
        // ȸ�� ���� �� ����
        transform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.25f).onComplete += () => 
        { 
            axeShooter.SpawnProjectile(axeShooter.transform.position, direction, (float)PhotonNetwork.Time + 0.05f);

            photonView.RPC("ShootAxe_RPC", RpcTarget.Others, axeShooter.transform.position, direction, (float)PhotonNetwork.Time + 0.05f);
        };

        // ���� �ִϸ��̼� �� ����
        yield return new WaitForSeconds(attackDuration);

        IsActionInProgress = false;
        axeObj.SetActive(true);
        OnActionCompleted?.Invoke();
    }

    // ���� ���� Ȱ��ȭ �޼���
    public void ActivateRange(bool active)
    {
        if (!photonView.IsMine) { return; }
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

    [PunRPC]
    private void ShootAxe_RPC(Vector3 StartPos, Vector3 direction, float execTime)
    {
        axeShooter.SpawnProjectile(StartPos, direction, execTime);
    }
}