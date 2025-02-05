using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : ManagerBase<UIManager>
{
    [SerializeField] GameObject lobbyUI;
    [SerializeField] GameObject LoadingUI;
    public RoomUI roomUI;

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
