using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public void OnEnable()
    {
        modelTrf.localRotation = Quaternion.Euler(startRotation);
    }

    public void Init(Vector3 targetPoint, float flyTime, PlayerAttack sender)
    {
        owner = sender;
        if (moveTweenCore != null) moveTweenCore.Kill();
        moveTweenCore = transform.DOMove(targetPoint, flyTime).SetEase(Ease.Linear);
        moveTweenCore.onComplete += () => ObjectPooler.Release("Axe", gameObject);
        if (rotateTweenCore != null) rotateTweenCore.Kill();
        rotateTweenCore = modelTrf.DOLocalRotate(new Vector3(0.0f, 810.0f, 0.0f), flyTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable) && other.gameObject != owner.gameObject)
        {
            damageable.Damage(owner.attackPower);
            moveTweenCore?.Complete();
            ObjectPooler.Release("Axe", gameObject);
        }
    }

    [Header("References")]
    public Transform modelTrf;
    PlayerAttack owner;
    private Vector3 startRotation = new Vector3(-30.0f, 0.0f, -90.0f);
    private TweenerCore<Vector3, Vector3, VectorOptions> moveTweenCore;
    private TweenerCore<Quaternion, Vector3, QuaternionOptions> rotateTweenCore;
}
