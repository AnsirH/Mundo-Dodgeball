using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Photon.Pun;
using UnityEngine;

public class Axe : MonoBehaviour, IProjectile
{
    public void Initialize(IPlayerContext context, float damage, Vector3 spawnPos, Vector3 direction, float execTime)
    {
        this.context = context;
        this.damage = damage;
        transform.position = spawnPos;
        Launch(direction, execTime);
    }

    private void Launch(Vector3 direction, float execTime)
    {
        // 시간 차이 계산
        float timeInterval = (float)PhotonNetwork.Time - execTime;
        float adjustedFlyTime = flyTime - timeInterval;

        // 위치 설정
        Vector3 targetPos = transform.position + direction * 10.0f;
        if (moveTweenCore != null) moveTweenCore.Kill();
        moveTweenCore = transform.DOMove(targetPos, adjustedFlyTime).SetEase(Ease.Linear);
        moveTweenCore.onComplete += () => ObjectPooler.Release("Axe", gameObject);

        // 회전 설정
        transform.rotation = Quaternion.LookRotation(direction);
        if (rotateTweenCore != null) rotateTweenCore.Kill();
        rotateTweenCore = modelTrf.DOLocalRotate(new Vector3(0.0f, 810.0f, 0.0f), adjustedFlyTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
    }

    public void OnHit(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable) && other.gameObject != context.Trf.gameObject)
        {
            damageable.Damage(damage);
            moveTweenCore?.Complete();
            ObjectPooler.Release("Axe", gameObject);
        }
    }

    public void OnEnable()
    {
        modelTrf.localRotation = Quaternion.Euler(startRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHit(other);
    }

    [Header("References")]
    public Transform modelTrf;


    private Vector3 startRotation = new Vector3(-30.0f, 0.0f, -90.0f);
    private TweenerCore<Vector3, Vector3, VectorOptions> moveTweenCore;
    private TweenerCore<Quaternion, Vector3, QuaternionOptions> rotateTweenCore;


    IPlayerContext context;
    float damage = 0.0f;
    float flyTime = 0.6f;
}
