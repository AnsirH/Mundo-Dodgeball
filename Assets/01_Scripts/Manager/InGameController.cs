using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameController : MonoBehaviour
{
    public static InGameController Instance { get; private set; }
    
    private void Awake()
    {
        // 이미 존재하는 인스턴스가 있고, 그것이 자신(this)이 아니라면
        // 중복 인스턴스를 제거
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // 자신의 인스턴스를 기록
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
