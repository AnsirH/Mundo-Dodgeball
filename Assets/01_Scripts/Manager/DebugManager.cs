
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private bool showNetworkInfo = true;
    [SerializeField] private bool showPlayerState = true;
    private readonly Color debugTextColor = new Color(0f, 0.5f, 0f); // £�� �ʷϻ�
    private GUIStyle debugStyle;

    private void OnGUI()
    {
        // GUI ��Ÿ�� �ʱ�ȭ
        debugStyle = new GUIStyle(GUI.skin.label);
        debugStyle.fontSize = 36; // �⺻ ��Ʈ ũ��(12)�� 3��

        // ���� GUI ���� ����
        Color originalColor = GUI.color;

        // �ؽ�Ʈ ���� ����
        GUI.color = debugTextColor;

        if (showNetworkInfo)
        {
            GUILayout.BeginArea(new Rect(10, 10, 700, 300));
            GUILayout.Label($"Network Status: {PhotonNetwork.NetworkClientState}", debugStyle);
            GUILayout.Label($"Room: {PhotonNetwork.CurrentRoom?.Name}", debugStyle);
            GUILayout.Label($"Players: {PhotonNetwork.CurrentRoom?.PlayerCount}", debugStyle);
            GUILayout.EndArea();
        }

        if (showPlayerState)
        {
            var players = Object.FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            foreach (var player in players)
            {
                GUILayout.BeginArea(new Rect(10, 320, 700, 300));
                GUILayout.Label($"Player: {player.name}", debugStyle);
                GUILayout.Label($"State: {player.PlayerState}", debugStyle);
                GUILayout.Label($"IsMine: {player.photonView.IsMine}", debugStyle);
                GUILayout.EndArea();
            }
        }

        // ���� GUI �������� ����
        GUI.color = originalColor;
    }
}