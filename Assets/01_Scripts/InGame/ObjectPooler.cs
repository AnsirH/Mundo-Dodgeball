using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler : MonoBehaviour
{
    static ObjectPooler inst;
    private void Awake() => inst = this;

    [SerializeField]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [SerializeField] Pool[] pools;
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
                    actionOnGet: obj => obj.SetActive(true),
                    actionOnRelease: obj => obj.SetActive(false),
                    actionOnDestroy: obj => Destroy(obj),
                    collectionCheck: false,
                    maxSize: newPool.size
                    ));
        }
    }

    private GameObject OnNewObjCreate(GameObject newObj)
    {
        GameObject obj = Instantiate(newObj, transform);
        obj.SetActive(false);
        return obj;
    }

    public static GameObject SpawnFromPool(string tag)
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
}
