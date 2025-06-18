using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler : MonoBehaviour
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

    public LocalPool[] localPools;
    private Dictionary<string, ObjectPool<GameObject>> localPoolDictionary;

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
        InitializePools();
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

