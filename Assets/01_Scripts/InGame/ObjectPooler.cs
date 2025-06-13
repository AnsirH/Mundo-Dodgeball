using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// ����, ����Ʈ�� ���� �Ͻ������� �����Ǵ� ������Ʈ�� ����
public class ObjectPooler : NetworkBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    [System.Serializable]
    public class LocalPool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [System.Serializable]
    public class NetworkPool
    {
        public string tag;
        public NetworkPrefabRef prefab;
        public int size;
    }

    // ������Ʈ Ǯ���� �����Ǵ� ������Ʈ���� �ν����� â���� ������ �־���� ��.
    public LocalPool[] localPools;
    private Dictionary<string, ObjectPool<GameObject>> localPoolDictionary;


    public NetworkPool[] networkPools;
    private Dictionary<string, ObjectPool<GameObject>> networkPoolDictionary { get; set; }

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

    // ������Ʈ Ǯ�� �ʱ�ȭ
    private void InitializePools()
    {
        // Ǯ ��ųʸ� ���� �� �ʱ�ȭ
        localPoolDictionary = new Dictionary<string, ObjectPool<GameObject>>();

        foreach (LocalPool pool in localPools)
        {
            ObjectPool<GameObject> objectPool = new ObjectPool<GameObject>(
                createFunc: () => CreatePooledObject(pool.prefab),
                actionOnGet: obj => OnGetObject(obj),
                actionOnRelease: obj => OnReleaseObject(obj),
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: false,
                maxSize: pool.size
            );

            localPoolDictionary.Add(pool.tag, objectPool);
        }


        networkPoolDictionary = new Dictionary<string, ObjectPool<GameObject>>();

        foreach (NetworkPool pool in networkPools)
        {
            ObjectPool<GameObject> objectPool = new ObjectPool<GameObject>(
                createFunc: () => CreatePooledObject(pool.prefab),
                actionOnGet: obj => OnGetObject(obj),
                actionOnRelease: obj => OnReleaseObject(obj),
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: false,
                maxSize: pool.size
            );

            networkPoolDictionary.Add(pool.tag, objectPool);
        }
    }

    /// <summary> ���� ������Ʈ �ʱ� ���� /// </summary>
    private GameObject CreatePooledObject(GameObject prefab)
    {
        var obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        return obj;
    }

    private GameObject CreatePooledObject(NetworkPrefabRef prefab)
    {
        var obj = Runner.Spawn(prefab).gameObject;
        obj.SetActive(false);
        return obj;
    }

    /// <summary> ������Ʈ Get �� �� ȣ�� /// </summary>
    private void OnGetObject(GameObject obj)
    {
        obj.SetActive(true);
    }

    /// <summary> ������Ʈ Release �� �� ȣ�� /// </summary>
    private void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    /// <summary> ������Ʈ Ǯ���� �������� /// </summary>
    public static GameObject Get(string tag)
    {
        if (Instance.localPoolDictionary.TryGetValue(tag, out var localPool))
        {
            return localPool.Get();
        }
        else if (Instance.networkPoolDictionary.TryGetValue(tag, out var networkPool))
        {
            return networkPool.Get();
        }
        else
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }
    }

    /// <summary> ������Ʈ Ǯ�� ��ȯ /// </summary>
    public static void Release(string tag, GameObject obj)
    {
        if (Instance.localPoolDictionary.TryGetValue(tag, out var localPool))
        {
            localPool.Release(obj);
        }
        else if (Instance.networkPoolDictionary.TryGetValue(tag, out var networkPool))
        {
            networkPool.Release(obj);
        }
        else
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            Destroy(obj);
        }
    }
}

