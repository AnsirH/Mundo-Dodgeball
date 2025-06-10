using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RoomView : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField roomNameInput;
    public TMP_InputField roomPasswordInput;
    public Button createButton;
    public Button leaveButton;
    public TMP_Text currentRoomText;
    public TMP_Text playerCountText;

    public event Action<string, string, string> OnCreateRoomRequested;
    public event Action OnLeaveRoomRequested;

    private void Start()
    {
        createButton.onClick.AddListener(() =>
        {
            string roomName = roomNameInput.text;
            string roomPassword = roomPasswordInput.text;
            if (!string.IsNullOrEmpty(roomName))
                OnCreateRoomRequested?.Invoke(roomName, roomPassword, "");
        });

        leaveButton.onClick.AddListener(() =>
        {
            OnLeaveRoomRequested?.Invoke();
        });
    }

    public void UpdateCurrentRoomText(string roomName)
    {
        currentRoomText.text = string.IsNullOrEmpty(roomName) ? "�� ����" : $"���� ���� ��: {roomName}";
    }

    public void UpdatePlayerCount(int count)
    {
        playerCountText.text = $"���� �ο�: {count}";
    }
}
