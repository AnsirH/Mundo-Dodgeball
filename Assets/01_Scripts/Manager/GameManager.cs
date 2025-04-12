using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region �̱���
    private static GameManager instance;

    // �ܺο��� instance�� ������ ���� �� ������Ƽ�� ���
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                // �� ������ ServerManager�� ã�ų�, 
                // �ʿ��ϴٸ� ��Ÿ�ӿ� �� ������Ʈ�� ������ ���� ����
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    // ���� ���� ServerManager�� ���ٸ� ���� ���� (���� ����)
                    GameObject obj = new GameObject(typeof(GameManager).Name);
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        // �̹� �ν��Ͻ��� ������, �ڱ� �ڽ��� �ı��� �ߺ��� ����
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // ������ �� �ν��Ͻ��� ���
        instance = this;

        // �� ��ȯ �� �ı����� �ʵ��� ����
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
    // Start is called before the first frame update
    public ReSourceManager resourceManager;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name != "MainScene" && !PopManager.instance.inGameSettingPop.gameObject.activeSelf)
            {
                PopManager.instance.inGameSettingPop.Open();
            }
        }
    }
    void Start()
    {
        
    }
}