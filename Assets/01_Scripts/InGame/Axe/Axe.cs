using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using UnityEngine;

public class Axe : MonoBehaviour, IProjectile
{
    public void Initialize(IPlayerContext context, Vector3 spawnPos, Vector3 direction)
    {
        this.context = context;

        model.transform.localRotation = Quaternion.Euler(startRotation);
        transform.position = spawnPos;

        model.SetActive(true);
        droppingModel.SetActive(false);

        Launch(direction);
    }

    private void Launch(Vector3 direction)
    {
        // 시간 차이 계산
        float timeInterval = 0.0f;/*Mathf.Max((float)PhotonNetwork.Time - execTime, 0.0f);*/
        float adjustedFlyTime = flyTime - timeInterval;
        float execTimeRatio = timeInterval / flyTime;

        // 10.0f은 이동거리( 변수로 수정해야 함 )
        Vector3 adjustedStartPosition = transform.position + direction * 10.0f * execTimeRatio;
        transform.position = adjustedStartPosition;
        // 위치 설정
        Vector3 targetPos = transform.position + direction * 10.0f;
        if (moveTweenCore != null) moveTweenCore.Kill();
        moveTweenCore = transform.DOMove(targetPos, adjustedFlyTime).SetEase(Ease.Linear);
        moveTweenCore.onComplete += () =>
        {
            StartCoroutine(DropAxe());
        };

        // 회전 설정
        transform.rotation = Quaternion.LookRotation(direction);
        if (rotateTweenCore != null) rotateTweenCore.Kill();
        rotateTweenCore = model.transform.DOLocalRotate(new Vector3(0.0f, 810.0f, 0.0f), adjustedFlyTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
    }

    private IEnumerator DropAxe()
    {
        model.gameObject.SetActive(false);
        droppingModel.gameObject.SetActive(true);
        droppingModel.transform.position = new Vector3(droppingModel.transform.position.x, 0.0f, droppingModel.transform.position.z);
        yield return new WaitForSeconds(1.0f);
        ObjectPooler.Release("Axe", gameObject);
    }

    public void OnHit(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable) && other.gameObject != context.Movement.gameObject)
        {
            damageable.TakeDamage(context);
            moveTweenCore?.Complete();
        }
    }

    public void OnEnable()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHit(other);
    }

    [Header("References")]
    public GameObject model;
    public GameObject droppingModel;


    private Vector3 startRotation = new Vector3(-30.0f, 0.0f, -90.0f);
    private TweenerCore<Vector3, Vector3, VectorOptions> moveTweenCore;
    private TweenerCore<Quaternion, Vector3, QuaternionOptions> rotateTweenCore;


    IPlayerContext context;
    float flyTime = 0.6f;
}
