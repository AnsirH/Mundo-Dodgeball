using Fusion;
using UnityEngine;

public class RoomModel : MonoBehaviour
{
    public SessionInfo CurrentSession { get; private set; }

    public void SetSessionInfo(SessionInfo session)
    {
        CurrentSession = session;
    }

    public void ClearSessionInfo()
    {
        CurrentSession = null;
    }

    public string GetRoomName()
    {
        return CurrentSession?.Name ?? "";
    }

    public int GetPlayerCount()
    {
        return CurrentSession?.PlayerCount ?? 0;
    }
}
