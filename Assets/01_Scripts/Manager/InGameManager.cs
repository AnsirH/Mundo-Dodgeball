using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }
    private void Awake()
    {
        // �̹� �����ϴ� �ν��Ͻ��� �ְ�, �װ��� �ڽ�(this)�� �ƴ϶��
        // �ߺ� �ν��Ͻ��� ����
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // �ڽ��� �ν��Ͻ��� ���
        Instance = this;

        // ���⼭ DontDestroyOnLoad(gameObject);�� ȣ������ ����!
        // �� ���� �ٲ�� �� ������Ʈ�� �ı��˴ϴ�.
    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("PlayerNew", Vector3.zero, Quaternion.identity);
    }
}
