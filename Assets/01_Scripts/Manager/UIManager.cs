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
        // 이미 존재하는 인스턴스가 있고, 그것이 자신(this)이 아니라면
        // 중복 인스턴스를 제거
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // 자신의 인스턴스를 기록
        instance = this;

        // 여기서 DontDestroyOnLoad(gameObject);를 호출하지 않음!
        // → 씬이 바뀌면 이 오브젝트는 파괴됩니다.
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
