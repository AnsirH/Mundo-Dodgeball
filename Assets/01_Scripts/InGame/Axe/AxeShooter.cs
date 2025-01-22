using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeShooter : MonoBehaviour
{
    public void SpawnAxe()
    {
        Instantiate(axePrefab, spawnPoint.position, Quaternion.LookRotation(transform.forward));
    }

    public GameObject axePrefab;
    public Transform spawnPoint;
}
