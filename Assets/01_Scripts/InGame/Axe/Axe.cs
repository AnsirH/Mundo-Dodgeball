using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public void OnEnable()
    {
        modelTrf.localRotation = Quaternion.Euler(startRotation);
    }

    public void FlyToTarget(Vector3 targetPoint, float flyTime)
    {
        transform.DOMove(targetPoint, flyTime).SetEase(Ease.Linear).onComplete += () => ObjectPooler.Release("Axe", gameObject);
        modelTrf.DOLocalRotate(new Vector3(0.0f, 810.0f, 0.0f), flyTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
    }

    public Transform modelTrf;

    private Vector3 startRotation = new Vector3(-30.0f, 0.0f, -90.0f);
}
