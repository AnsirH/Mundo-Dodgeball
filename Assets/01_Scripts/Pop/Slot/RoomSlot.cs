using Fusion;
using TMPro;
using UnityEngine;

public class RoomSlot : MonoBehaviour
{
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text personnelText;
    [SerializeField] GameObject secretIcon;
    public SessionInfo roomInfo;

    public void init(string _name, int _now, int _max, bool _isSc)
    {
        secretIcon.SetActive(_isSc);
        roomNameText.text = GetCleanRoomName(_name);
        personnelText.text = _now + "/" + _max;
    }

    public void init(SessionInfo info)
    {
        roomInfo = info;

        secretIcon.SetActive(GetOnSecretIcon(info.Name));
        roomNameText.text = GetCleanRoomName(info.Name);
        personnelText.text = info.PlayerCount + "/" + info.MaxPlayers;
    }

    public void ClickGetRoomId()
    {
        ServerManager.Instance.roomController.model.SetSessionInfo(roomInfo);
        Debug.Log(roomInfo.Name);
    }

    private string GetCleanRoomName(string rawName)
    {
        const string marker = "[03%14]";
        int markerIndex = rawName.IndexOf(marker);
        if (markerIndex > 0)
        {
            return rawName.Substring(0, markerIndex);
        }
        return rawName;
    }
    private bool GetOnSecretIcon(string rawName)
    {
        const string marker = "[01%01]";
        int markerIndex = rawName.IndexOf(marker);

        if (markerIndex >= 0)
        {
            string passwordPart = rawName.Substring(markerIndex + marker.Length);
            return !string.IsNullOrEmpty(passwordPart);
        }

        return false;
    }
}
