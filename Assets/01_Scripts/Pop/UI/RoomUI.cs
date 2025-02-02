using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUI : MonoBehaviour
{
    public void RoomExit()
    {
        ServerManager.Instance.roomManager.LeaveRoom();
    }
}
