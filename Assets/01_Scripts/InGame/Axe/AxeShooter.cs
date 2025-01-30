using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeShooter : MonoBehaviour
{
    public Vector3 targetDirection;

    public void SpawnAxe()
    {
        GameObject axeObj = ObjectPooler.Get("Axe");
        axeObj.transform.position = spawnPoint.position;
        axeObj.transform.rotation = Quaternion.LookRotation(targetDirection);
    }
    public Transform spawnPoint;
}
