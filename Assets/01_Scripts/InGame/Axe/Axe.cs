using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Transactions;
using UnityEngine;

public class Axe : MonoBehaviour, IProjectile
{
    public void Initialize(IPlayerContext context, float damage, Vector3 spawnPos)
    {
        this.context = context;
        this.damage = damage;
        transform.position = spawnPos;
    }

    public void Launch(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
        Vector3 targetPos = transform.position + direction;
        if (moveTweenCore != null) moveTweenCore.Kill();
        moveTweenCore = transform.DOMove(targetPos, flyTime).SetEase(Ease.Linear);
        moveTweenCore.onComplete += () => ObjectPooler.Release("Axe", gameObject);
        if (rotateTweenCore != null) rotateTweenCore.Kill();
        rotateTweenCore = modelTrf.DOLocalRotate(new Vector3(0.0f, 810.0f, 0.0f), flyTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
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
