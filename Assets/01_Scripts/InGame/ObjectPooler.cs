using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// 도끼, 이펙트와 같이 일시적으로 생성되는 오브젝트를 관리
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

    // 오브젝트 풀러로 관리되는 오브젝트들은 인스펙터 창에서 정보를 넣어줘야 함.
    public LocalPool[] localPools;
    private Dictionary<string, ObjectPool<GameObject>> localPoolDictionary;


    public NetworkPool[] networkPools;
    private Dictionary<string, ObjectPool<GameObject>> networkPoolDictionary;

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

    // 오브젝트 풀러 초기화
    private void InitializePools()
    {
        // 풀 딕셔너리 생성 및 초기화
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

            localPoolDictionary.Add(pool.tag, objectPool);
        }
    }

    /// <summary> 로컬 오브젝트 초기 생성 /// </summary>
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

    /// <summary> 오브젝트 Get 할 때 호출 /// </summary>
    private void OnGetObject(GameObject obj)
    {
        obj.SetActive(true);
    }

    /// <summary> 오브젝트 Release 할 때 호출 /// </summary>
    private void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    /// <summary> 오브젝트 풀에서 가져오기 /// </summary>
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

    /// <summary> 오브젝트 풀로 반환 /// </summary>
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

