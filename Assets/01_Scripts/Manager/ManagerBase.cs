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
                // �̹� ���� �ν��Ͻ��� �ִ��� Ȯ��
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    // �� ������Ʈ�� ���� T ������Ʈ�� �ٿ��ش�.
                    var singletonObject = new GameObject(typeof(T).Name + " (Singleton)");
                    _instance = singletonObject.AddComponent<T>();

                    // (����) �ٸ� ������ �ı����� �ʵ��� �Ϸ��� DontDestroyOnLoad ����
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // �̹� �����ϴ� �ν��Ͻ��� �ִٸ� �ڱ� �ڽ��� �ı�
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
