using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }
    private void Awake()
    {
        // 이미 존재하는 인스턴스가 있고, 그것이 자신(this)이 아니라면
        // 중복 인스턴스를 제거
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // 자신의 인스턴스를 기록
        Instance = this;

        // 여기서 DontDestroyOnLoad(gameObject);를 호출하지 않음!
        // → 씬이 바뀌면 이 오브젝트는 파괴됩니다.
    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("PlayerNew", Vector3.zero, Quaternion.identity);
    }
}
