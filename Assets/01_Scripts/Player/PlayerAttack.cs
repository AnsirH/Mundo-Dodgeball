using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviourPunCallbacks, IPunObservable, IPlayerComponent, IPlayerAction
{
    private IPlayerContext context;

    public float attackPower = 80.0f;
    private float attackDuration = 0.75f;
    private bool axeRangeToggle = false;

    private Coroutine currentAttackRoutine;

    [Header("References")]
    // ���� �߻�ü
    [SerializeField] private AxeShooter axeShooter;

    // ĳ���� ���� �𵨸�
    [SerializeField] private GameObject axeObj;

    #region IPlayerAction Implementation
    public event Action OnActionCompleted;

    public bool IsActionInProgress { get; private set; } = false;

    public void ExecuteAction()
    {
        if (IsActionInProgress || !axeRangeToggle) return;

        ActivateRange(false);
        Vector3? targetPoint = context.GetMousePosition();
        if (targetPoint.Value == null) return;
        Vector3 direction = (targetPoint.Value - context.Pos).normalized;
        direction.y = 0.0f;
        transform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.25f).onComplete += () => { SpawnAxe(); };

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

    #region Animation Event Functions
    public void SpawnAxe()
    {
        axeShooter.ShootAxe(this);
        axeObj.SetActive(false);
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
        axeRangeToggle = active;
        axeShooter.ShowRange(active);
    }

    public void CancelAttack()
    {
        ActivateRange(false);

        // ȸ�� ����
        transform.DOKill();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(axeShooter.targetPoint);
        else
            axeShooter.targetPoint = (Vector3)stream.ReceiveNext();
    }

}