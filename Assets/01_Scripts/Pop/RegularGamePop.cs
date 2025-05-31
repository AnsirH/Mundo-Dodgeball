using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegularGamePop : MonoBehaviour
{
    [SerializeField] TMP_InputField searchInput;
    [SerializeField] Button searchBtn;
    [SerializeField] Button createBtn;
    [SerializeField] Button joinBtn;
    [SerializeField] List<RoomSlot> roomList = new List<RoomSlot>();

    public void SetRoomListSlot(List<SessionInfo> _roomList)
    {
        for(int i = 0; i < roomList.Count; i++)
        {
            roomList[i].gameObject.SetActive(false);
            if(_roomList.Count > i && _roomList[i].PlayerCount > 0)
            {
                roomList[i].gameObject.SetActive(true);
                roomList[i].init(_roomList[i]);
            }
        }
    }
}
