using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ��� ������ ���� ����
    private static SteamManager instance;

    // Steam API �ʱ�ȭ ���� ����
    private bool initialized = false;

    // �ܺο��� Steam API�� ���� Ȱ��ȭ(�ʱ�ȭ)�Ǿ����� ������ Ȯ���� �� �ִ� ������Ƽ
    public static bool Initialized
    {
        get
        {
            // �̱��� ������Ʈ�� �����ϰ�, �ʱ�ȭ�� �������� �˻�
            return instance != null && instance.initialized;
        }
    }

    // ��ũ��Ʈ�� ����(������Ʈ�� �ε�)�� �� �ڵ����� ȣ��
    void Awake()
    {
        // ���� �̹� instance�� �����Ѵٸ�(= �ٸ� SteamManager�� �ִٸ�),
        // �ߺ� ������ ���� ���� �� ������Ʈ�� ����
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // �׷��� �ʴٸ�, �� ������Ʈ�� �̱������� ����
        instance = this;

        // �� ��ȯ �ÿ��� �ı����� �ʵ��� ���� (Steam API�� ���� ��ü���� ������ �ʿ�)
        DontDestroyOnLoad(gameObject);

        try
        {
            // Steamworks.Net ���: Steam Ŭ���̾�Ʈ�� ���� �õ�
            // - ���� ����Ǹ� true ��ȯ, ���� �� false
            initialized = SteamAPI.Init();
            if (!initialized)
            {
                // ���� �� ���� �޽��� ���
                Debug.LogError("SteamAPI.Init() failed.");
            }
        }
        catch (System.DllNotFoundException e)
        {
            // steam_api.dll �Ǵ� libsteam_api.so ������ ã�� ���� ���
            Debug.LogError("[Steamworks] Could not load steam_api: " + e);
            return;
        }
    }

    // �� �����Ӹ��� Unity�� �ڵ����� ȣ��
    void Update()
    {
        // Steam API�� ���������� �ʱ�ȭ�Ǿ��� ���� �ݹ� ����
        if (initialized)
        {
            // RunCallbacks()�� Steamworks���� �̺�Ʈ(����, ��� ������Ʈ ��)��
            // ���޹޴� �ٽ� �޼���. �� �����Ӹ��� ȣ�����־�� �ݹ��� ���� ó����
            SteamAPI.RunCallbacks();
        }
    }

    // ������Ʈ�� ���ŵ� ��(�� ���� Ȥ�� ���� ���� ��) �ڵ����� ȣ��
    void OnDestroy()
    {
        // �̱��� �ν��Ͻ��� ������ ������Ʈ���� Ȯ��
        if (instance == this)
        {
            // Steam�� �ʱ�ȭ�Ǿ� �־��ٸ�, �����ϰ� Shutdown() ȣ��
            if (initialized)
            {
                SteamAPI.Shutdown();
            }
            // �̱��� ���� ����
            instance = null;
        }
    }
}
