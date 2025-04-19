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

    // 애니메이션 트리거 전송
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

        // 회전 종료
        transform.DOKill();
    }
}