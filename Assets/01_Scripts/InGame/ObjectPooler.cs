using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler : NetworkBehaviour
{
    public static ObjectPooler Instance { get; private set; }

    // 비동기화 오브젝트 풀 정보
    [System.Serializable]
    public class LocalPool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    // 동기화 오브젝트 풀 정보
    [System.Serializable]
    public class NetworkPool
    {
        public string tag;
        public NetworkPrefabRef prefab;
        public int size;
    }

    public LocalPool[] localPools;
    private Dictionary<string, ObjectPool<GameObject>> localPoolDictionary;

    public NetworkPool[] networkPools;
    private Dictionary<string, Queue<NetworkObject>> networkPoolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializePools()
    {
        // 기존 로컬 풀 오브젝트 모두 삭제
        if (localPoolDictionary != null)
        {
            foreach (var pool in localPoolDictionary.Values)
            {
                // ObjectPool에는 직접 오브젝트 목록이 없으므로, 풀에서 모두 꺼내서 삭제
                var tempList = new List<GameObject>();
                while (pool.CountActive > 0)
                {
                    var obj = pool.Get();
                    tempList.Add(obj);
                }
                foreach (var obj in tempList)
                {
                    Destroy(obj);
                }
            }
            localPoolDictionary.Clear();
        }

        // 기존 네트워크 풀 오브젝트 모두 삭제
        if (networkPoolDictionary != null)
        {
            foreach (var queue in networkPoolDictionary.Values)
            {
                while (queue.Count > 0)
                {
                    var netObj = queue.Dequeue();
                    if (netObj != null && Runner != null)
                    {
                        Runner.Despawn(netObj);
                    }
                    else if (netObj != null)
                    {
                        Destroy(netObj.gameObject);
                    }
                }
            }
            networkPoolDictionary.Clear();
        }
        // 로컬 풀 초기화
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

        // 네트워크 풀 초기화 (Queue 사용)
        networkPoolDictionary = new Dictionary<string, Queue<NetworkObject>>();
        foreach (NetworkPool pool in networkPools)
        {
            var queue = new Queue<NetworkObject>();
            for (int i = 0; i < pool.size; i++)
            {
                var netObj = Runner.Spawn(pool.prefab, Vector3.zero, Quaternion.identity);
                netObj.gameObject.SetActive(false);
                queue.Enqueue(netObj);
            }
            networkPoolDictionary.Add(pool.tag, queue);
        }
    }
    private GameObject CreatePooledObject(GameObject prefab)
    {
        var obj = Instantiate(prefab, transform);
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

    // 네트워크 오브젝트 Get
    public static NetworkObject GetNetwork(string tag, Vector3 position, Quaternion rotation)
    {
        if (Instance.networkPoolDictionary.TryGetValue(tag, out var queue))
        {
            if (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                obj.transform.SetPositionAndRotation(position, rotation);
                obj.gameObject.SetActive(true);
                return obj;
            }
            else
            {
                Debug.LogWarning($"Network pool with tag {tag} is empty.");
                return null;
            }
        }
        else
        {
            Debug.LogWarning($"Network pool with tag {tag} doesn't exist.");
            return null;
        }
    }

    // 네트워크 오브젝트 Release
    public static void ReleaseNetwork(string tag, NetworkObject obj)
    {
        obj.gameObject.SetActive(false);
        if (Instance.networkPoolDictionary.TryGetValue(tag, out var queue))
        {
            queue.Enqueue(obj);
        }
        else
        {
            Debug.LogWarning($"Network pool with tag {tag} doesn't exist.");
            Instance.Runner.Despawn(obj);
        }
    }

    // 기존 로컬 오브젝트 Get/Release는 그대로 유지
    public static GameObject GetLocal(string tag)
    {
        if (Instance.localPoolDictionary.TryGetValue(tag, out var pool))
            return pool.Get();
        Debug.LogWarning($"Local pool with tag {tag} doesn't exist.");
        return null;
    }

    public static void ReleaseLocal(string tag, GameObject obj)
    {
        if (Instance.localPoolDictionary.TryGetValue(tag, out var pool))
            pool.Release(obj);
        else
        {
            Debug.LogWarning($"Local pool with tag {tag} doesn't exist.");
            Destroy(obj);
        }
    }
}

