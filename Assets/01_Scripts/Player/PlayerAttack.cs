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
    // 도끼 발사체
    [SerializeField] private AxeShooter axeShooter;

    // 캐릭터 도끼 모델링
    [SerializeField] private GameObject axeObj;

    #region IPlayerAction Implementation
    public event Action OnActionCompleted;

    public bool IsActionInProgress { get; private set; } = false;

    public bool CanExecuteAction => axeShooter.CanShoot && axeShooter.IsRangeActive;

    public bool Controllable { get; set; } = true;

    public void ExecuteAction()
    {
        if (IsActionInProgress || !axeShooter.IsRangeActive) return;

        // 공격 범위 디스플레이 비활성화
        ActivateRange(false);

        // 공격 로직 실행
        currentAttackRoutine = StartCoroutine(AttackRoutine());

        // 공격 실행 활성화
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

    // 애니메이션 트리거 전송
    void PlayAttackAnimation()
    {
        context.Anim.SetTrigger("Attack");
    }

    // 공격 코루틴. 시간 지나면 완료 이벤트 발행
    private IEnumerator AttackRoutine()
    {
        // 마우스 위치 확인
        if (!context.MousePositionGetter.ClickPoint.HasValue) yield break;

        // 공격 방향 계산
        Vector3 direction = (context.MousePositionGetter.ClickPoint.Value - context.Pos).normalized;
        direction.y = 0.0f;

        // 공격 애니메이션
        PlayAttackAnimation();

        // 공격 방향으로 회전
        // 회전 종료 시 공격
        transform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.25f).onComplete += () => 
        { 
            axeShooter.SpawnProjectile(axeShooter.transform.position, direction, (float)PhotonNetwork.Time + 0.05f);

            photonView.RPC("ShootAxe_RPC", RpcTarget.Others, axeShooter.transform.position, direction, (float)PhotonNetwork.Time + 0.05f);
        };

        // 공격 애니메이션 및 로직
        yield return new WaitForSeconds(attackDuration);

        IsActionInProgress = false;
        axeObj.SetActive(true);
        OnActionCompleted?.Invoke();
    }

    // 도끼 궤적 활성화 메서드
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

        // 회전 종료
        transform.DOKill();
    }

    [PunRPC]
    private void ShootAxe_RPC(Vector3 StartPos, Vector3 direction, float execTime)
    {
        axeShooter.SpawnProjectile(StartPos, direction, execTime);
    }
}