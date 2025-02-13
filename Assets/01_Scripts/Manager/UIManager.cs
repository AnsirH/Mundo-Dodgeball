using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject lobbyUI;
    [SerializeField] GameObject LoadingUI;
    public RoomUI roomUI;
    public static UIManager instance { get; private set; }
    private void Awake()
    {
        // �̹� �����ϴ� �ν��Ͻ��� �ְ�, �װ��� �ڽ�(this)�� �ƴ϶��
        // �ߺ� �ν��Ͻ��� ����
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // �ڽ��� �ν��Ͻ��� ���
        instance = this;

        // ���⼭ DontDestroyOnLoad(gameObject);�� ȣ������ ����!
        // �� ���� �ٲ�� �� ������Ʈ�� �ı��˴ϴ�.
    }
    void Start()
    {
        
    }
    public void ChangeRoomUI()
    {
        roomUI.gameObject.SetActive(true);
        lobbyUI.SetActive(false);
        PopManager.instance.AllPopClose();
    }
    public void ChangeLobbyUI()
    {
        roomUI.gameObject.SetActive(false);
        lobbyUI.SetActive(true);
        PopManager.instance.AllPopClose();
    }
    public void SetLoadingUI(bool on)
    {
        LoadingUI.SetActive(on);
    }
}
