using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyGame.Utils;
using UnityEngine.SceneManagement;

public class IngameController : MonoBehaviourPun
{
    #region �̱��� ����
    public static IngameController Instance { get; private set; }

    private void Awake()
    {
        // �̹� �����ϴ� �ν��Ͻ��� �ְ�, �װ��� �ڽ�(this)�� �ƴ϶��
        // �ߺ� �ν��Ͻ��� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // �ڽ��� �ν��Ͻ��� ���
            Instance = this;
        }

        
        if (FindFirstObjectByType<ObjectPooler>() == null)
        {
            Debug.LogError("There is no ObjectPooler in scene");
            return;
        }    

        UIManager.instance.ChangeGame(false);

        StartCoroutine(StartGameProcess());

    }
    #endregion
    public int playerAmount = 2;
    private List<int> PunPlayerScores = new List<int>();
    IEnumerator StartGameProcess()
    {
        // ��� �÷��̾ �����ߴ��� Ȯ��
        yield return StartCoroutine(CheckPlayerConnected());

        if (PhotonNetwork.IsMasterClient)
        {
            // ������ �غ� �Ǿ����� ��� Ŭ���̾�Ʈ���� �˸�
            photonView.RPC(nameof(CreatePlayerCharacter_RPC), RpcTarget.All);
        }

        // ĳ���� ������ �Ϸ�Ǿ����� Ȯ�� �� ���� ����
        yield return StartCoroutine(CheckPlayersCharacterSpawned());

        // ���� ����
        StartGame();
    }

    /// <summary>
    /// �÷��̾ ��� �����Ͽ����� Ȯ��
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckPlayerConnected()
    {
        WaitForSeconds delay = new(1.0f);

        while (PhotonNetwork.PlayerList.Length < playerSpawnPoints.Length)
        {
            yield return delay;
        }
    }

    [PunRPC]
    private void CreatePlayerCharacter_RPC()
    {
        // �÷��̾� ĳ���� ����
        CreatePlayerCharacter();
    }

    private void CreatePlayerCharacter()
    {
        // �÷��̾��� ActorNumber�� ��ġ ����
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int spawnIndex = actorNumber - 1;

        PhotonNetwork.Instantiate("Player", playerSpawnPoints[spawnIndex].position, playerSpawnPoints[spawnIndex].rotation);
    }

    /// <summary>
    /// ��� �÷��̾��� ĳ���Ͱ� �����Ǿ����� Ȯ��
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckPlayersCharacterSpawned()
    {
        WaitForSeconds delay = new(0.5f);

        // ��� ĳ���Ͱ� ������ ������ ���
        while (true)
        {
            var foundControllers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);

            // �ʿ��� �� ��ŭ �� ã������ ���� �� ����
            if (foundControllers.Length >= playerSpawnPoints.Length)
            {
                playerControllers = new List<PlayerController>(new PlayerController[playerSpawnPoints.Length]);

                foreach (var controller in foundControllers)
                {
                    PhotonView view = controller.GetComponent<PhotonView>();
                    if (view != null)
                    {
                        int actorNum = view.Owner.ActorNumber;
                        int index = actorNum - 1;

                        if (index >= 0 && index < playerControllers.Count)
                        {
                            playerControllers[index] = controller;
                            (controller as IMousePositionGetter).SetClickableGroundLayer($"Ground_{index+1}");
                        }
                        else
                        {
                            Debug.LogWarning($"ActorNumber {actorNum}�� ���� �ε��� ������ ���.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("PhotonView�� ���� ��Ʈ�ѷ� �߰�.");
                    }
                }

                // ���� ���������� �Ҵ�ƴ��� Ȯ��
                if (playerControllers.All(c => c != null))
                    break;
            }

            yield return delay;
        }        
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void StartGame()
    {
        foreach (PlayerController player in playerControllers)
        {
            player.enabled = true;
        }
        ingameUIController.Init();
        ingameUIController.OnRoundPanel(ServerManager.Instance.roomManager.GetCurrentRound());
    }
    [PunRPC]
    void ReloadSceneRPC()
    {
        // Photon ���� ������ ä ���� �� ���ε�
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    /// <summary>�÷��̾� ĳ���� �迭</summary>
    public List<PlayerController> playerControllers = new List<PlayerController>(2);
    /// <summary>�ΰ��� UI ��Ʈ�ѷ�</summary>
    public IngameUIController ingameUIController;

    public Transform[] playerSpawnPoints = new Transform[2];

    public Ground ground;
}