
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private bool showNetworkInfo = true;
    [SerializeField] private bool showPlayerState = true;
    private readonly Color debugTextColor = new Color(0f, 0.5f, 0f); // 짙은 초록색
    private GUIStyle debugStyle;

    private void OnGUI()
    {
        // GUI 스타일 초기화
        debugStyle = new GUIStyle(GUI.skin.label);
        debugStyle.fontSize = 36; // 기본 폰트 크기(12)의 3배

        // 현재 GUI 색상 저장
        Color originalColor = GUI.color;

        // 텍스트 색상 설정
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

        // 원래 GUI 색상으로 복원
        GUI.color = originalColor;
    }
}