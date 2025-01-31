using ExitGames.Client.Photon.StructWrapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AxeShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] RangeDisplayer rangeDisplayer;

    public Vector3 targetPoint;
    public float flyTime = 0.5f;
    public float flyDistance = 5.0f;

    public void SpawnAxe()
    {
        GameObject axeObj = ObjectPooler.Get("Axe");
        axeObj.transform.position = transform.position;

        targetPoint.y = transform.position.y;
        Vector3 direction = (targetPoint - transform.position).normalized;
        axeObj.transform.rotation = Quaternion.LookRotation(direction);

        Vector3 destination = transform.position + direction * flyDistance;
        axeObj.GetComponent<Axe>().FlyToTarget(destination, flyTime);
    }

    public void DisplayRange(Vector3 direction)
    {
        rangeDisplayer.UpdateRange(direction, flyDistance);
    }

    public void ShowRange(bool active)
    {
        rangeDisplayer.gameObject.SetActive(active);
    }
}
