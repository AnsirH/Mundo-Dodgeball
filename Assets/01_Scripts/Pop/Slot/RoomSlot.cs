using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class RoomSlot : MonoBehaviour
{
    private string roomID;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text personnelText;
    [SerializeField] GameObject secretIcon;
    public void init(string _name, int _now, int _max, bool _isSc)
    {
        roomID = _name;
        secretIcon.SetActive(_isSc);
        roomNameText.text = _name;
        personnelText.text = _now + "/" + _max;
    }
    public void ClickGetRoomId()
    {
         ServerManager.Instance.roomManager.joinRoomId = roomID;
    }
}
