using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class GameManager : MonoBehaviour
{
    #region �̱���
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(GameManager).Name);
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    public ReSourceManager resourceManager;
    public GameObject playerPrefab;
    public int maxScore = 3;
    public string gameSceneName = "GeneralModeScene";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name != "MainScene" && !IngameController.Instance.ingameUIController.InGameSettingPop.gameObject.activeSelf)
            {
                IngameController.Instance.ingameUIController.InGameSettingPop.Open();
            }
        }
    }

    public void OnEndGame(PlayerRef winnerRef)
    {
        Debug.Log($"[Fusion] ����: {winnerRef}");
        Invoke(nameof(LeaveRoom), 3f);
    }

    private void LeaveRoom()
    {
        var runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
        {
            runner.Shutdown();
            SceneManager.LoadScene("MainScene");
        }
    }
}
