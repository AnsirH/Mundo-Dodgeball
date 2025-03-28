using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] RangeDisplayer rangeDisplayer;

    public float currentCoolTime = 0.0f;
    public float maxCoolTime = 2.5f;

    public Vector3 targetPoint;
    public float flyTime = 0.5f;
    public float flyDistance = 5.0f;

    public bool CanShoot { get { return currentCoolTime <= 0.0f; } }

    private void Update()
    {
        if (!CanShoot)
            Cooldown();
    }

    private void Cooldown()
    {
        if (currentCoolTime > 0.0f)
        {
            currentCoolTime -= Time.deltaTime;
        }
        else
        {
            currentCoolTime = 0.0f;
        }
    }

    public void ShootAxe(PlayerAttack sender)
    {
        if (!CanShoot)
        {
            return;
        }

        targetPoint.y = transform.position.y;
        Vector3 direction = (targetPoint - transform.position).normalized;

        //GameObject axeObj = ObjectPooler.Instance.Instantiate("Axe", transform.position, Quaternion.LookRotation(direction));
        GameObject axeObj = ObjectPooler.Get("Axe");
        axeObj.transform.position = transform.position;
        axeObj.transform.rotation = Quaternion.LookRotation(direction);

        Vector3 destination = transform.position + direction * flyDistance;
        axeObj.GetComponent<Axe>().Init(destination, flyTime, sender);

        currentCoolTime = maxCoolTime;
    }

    public void DisplayRange(Vector3 targetPoint)
    {
        Vector3 direction = targetPoint - transform.position;
        direction.y = 0.0f;
        rangeDisplayer.UpdateRange(direction, flyDistance);
    }

    public void ShowRange(bool active)
    {
        rangeDisplayer.gameObject.SetActive(active);
    }

#if UNITY_EDITOR

    public void OnGUI()
    {
        GUILayout.TextField($"Current Cool Time : {currentCoolTime}");
    }

#endif
}
