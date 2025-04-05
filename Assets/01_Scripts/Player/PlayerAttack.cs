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
    // 도끼 발사체
    [SerializeField] private AxeShooter axeShooter;

    // 캐릭터 도끼 모델링
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

    // 공격 코루틴. 시간 지나면 완료 이벤트 발행
    private IEnumerator AttackRoutine()
    {
        // 공격 애니메이션 및 로직
        yield return new WaitForSeconds(attackDuration);

        IsActionInProgress = false;
        axeObj.SetActive(true);
        OnActionCompleted?.Invoke();
    }

    // 도끼 궤적 활성화 메서드
    public void ActivateRange(bool active)
    {
        axeRangeToggle = active;
        axeShooter.ShowRange(active);
    }

    public void CancelAttack()
    {
        ActivateRange(false);

        // 회전 종료
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