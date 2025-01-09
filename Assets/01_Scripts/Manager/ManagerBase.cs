using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                // 이미 씬에 인스턴스가 있는지 확인
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    // 새 오브젝트를 만들어서 T 컴포넌트를 붙여준다.
                    var singletonObject = new GameObject(typeof(T).Name + " (Singleton)");
                    _instance = singletonObject.AddComponent<T>();

                    // (선택) 다른 씬에서 파괴되지 않도록 하려면 DontDestroyOnLoad 적용
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // 이미 존재하는 인스턴스가 있다면 자기 자신은 파괴
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
