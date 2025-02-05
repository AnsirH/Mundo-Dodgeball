using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomUI : MonoBehaviour
{
    public TMP_Text leftPlayerText;
    public TMP_Text rightPlayerText;
    public void RoomExit()
    {
        ServerManager.Instance.roomManager.LeaveRoom();
    }
}
