using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public Pool[] pools;
    private Dictionary<string, ObjectPool<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, ObjectPool<GameObject>>();

        foreach (Pool pool in pools)
        {
            ObjectPool<GameObject> objectPool = new ObjectPool<GameObject>(
                createFunc: () => CreatePooledObject(pool.prefab),
                actionOnGet: obj => OnGetObject(obj),
                actionOnRelease: obj => OnReleaseObject(obj),
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: false,
                maxSize: pool.size
            );

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    private GameObject CreatePooledObject(GameObject prefab)
    {
        var obj = Instantiate(prefab, transform);
        obj.GetComponent<PhotonView>().ViewID = PhotonNetwork.AllocateViewID(false);
        obj.SetActive(false);
        return obj;
    }

    private void OnGetObject(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    public static GameObject Get(string tag)
    {
        if (Instance.poolDictionary.TryGetValue(tag, out var pool))
        {
            return pool.Get();
        }
        else
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }
    }

    public static void Release(string tag, GameObject obj)
    {
        if (Instance.poolDictionary.TryGetValue(tag, out var pool))
        {
            pool.Release(obj);
        }
        else
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            Object.Destroy(obj);
        }
    }
}
