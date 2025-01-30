using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(FlyCoroutine());
    }


    private IEnumerator FlyCoroutine()
    {
        Vector3 destination = transform.position + transform.forward * flyDistance;

        while (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            transform.Translate(transform.forward * flySpeed * Time.deltaTime, Space.World);
            yield return null;
        }

        ObjectPooler.Release("Axe", gameObject);
    }

    public float flySpeed = 5.0f;
    public float flyDistance = 5.0f;
}
