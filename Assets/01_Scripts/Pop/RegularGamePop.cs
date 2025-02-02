using Photon.Realtime;
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
    public void SetRoomListSlot(List<RoomInfo> _roomList)
    {
        for(int i = 0; i < roomList.Count; i++) 
        {
            roomList[i].gameObject.SetActive(false);
            if(_roomList.Count > i)
            {
                roomList[i].gameObject.SetActive(true);
                RoomInfo info = _roomList[i];
                bool isSc = false;
                if (info.CustomProperties.ContainsKey("password"))
                {
                    isSc = true;
                }
                roomList[i].init(info.Name, info.PlayerCount, info.MaxPlayers, isSc);
            }
        }
    }
}
