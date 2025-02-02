using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : ManagerBase<UIManager>
{
    [SerializeField] GameObject lobbyUI;
    [SerializeField] RoomUI roomUI;
    void Start()
    {
        
    }
    public void ChangeRoomUI()
    {
        roomUI.gameObject.SetActive(true);
        lobbyUI.SetActive(false);
        PopManager.instance.AllPopClose();
    }
}
