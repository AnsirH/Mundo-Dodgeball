using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler : MonoBehaviour
{
    static ObjectPooler inst;
    private void Awake() => inst = this;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public Pool[] pools;
    public Dictionary<string, ObjectPool<GameObject>> poolDictionary = new();

    private void Start()
    {
        for (int i = 0; i < pools.Length; i++)
        {
            Pool pool = pools[i];
            AddPool(pool);
        }
    }

    private void AddPool(Pool newPool)
    {
        if (!poolDictionary.ContainsKey(newPool.tag))
        {
            poolDictionary.Add(newPool.tag,
                new ObjectPool<GameObject>(
                    createFunc: () => OnNewObjCreate(newPool.prefab),
                    actionOnGet: obj => OnGetAction(obj),
                    actionOnRelease: obj => OnReleaseAction(obj),
                    actionOnDestroy: obj => OnDestroyAction(obj),
                    collectionCheck: false,
                    maxSize: newPool.size
                    ));
        }
    }

    private void OnGetAction(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnReleaseAction(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnDestroyAction(GameObject obj)
    {
        Destroy(obj);
    }

    private GameObject OnNewObjCreate(GameObject newObj)
    {
        GameObject obj = Instantiate(newObj, transform);
        obj.SetActive(false);
        return obj;
    }

    public static GameObject Get(string tag)
    {
        if (inst.poolDictionary.TryGetValue(tag, out ObjectPool<GameObject> pool))
        {
            return pool.Get();
        }
        else
        {
            return null;
        }
    }

    public static void Release(string tag, GameObject obj)
    {
        if (inst.poolDictionary.TryGetValue(tag, out ObjectPool<GameObject> pool))
        {
            pool.Release(obj);
        }
        else
        {
            return;
        }
    }
}
