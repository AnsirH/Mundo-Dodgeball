using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameController : MonoBehaviour
{
    public static InGameController Instance { get; private set; }
    
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

        UIManager.instance.ChangeGame(false);
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

    void Start()
    {
        playerControllers = FindObjectsOfType<PlayerController>();
    }

    private PlayerController[] playerControllers;
}
