// NetworkPlayer.cs
using Fusion;
using UnityEngine;
public class NetworkPlayer : NetworkBehaviour
{
    //자동으로 동기화되는 닉네임
    [Networked] public string Nickname { get; set; }

    //자동으로 동기화되는 레디 여부
    [Networked] public bool IsReady { get; set; }

    private string _prevNickname = "";
    private bool? _prevReady = false;

    //플레이어가 Network에 Spawn 되었을 때 호출됨
    public override void Spawned()
    {
        //본인이 조작하는 객체일 경우에만 값 설정
        if (Object.HasInputAuthority)
        {
            // 입장 시에는 기본적으로 레디 상태가 아님
            IsReady = false;
            _prevReady = false;
            Debug.Log($"[PlayerInfo] 내 닉네임: {Nickname}");
            //ServerManager.Instance.roomController.UpdateLobbyUI();
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (_prevNickname != Nickname || _prevReady != IsReady)
        {
            _prevNickname = Nickname;
            _prevReady = IsReady;
            if (HasStateAuthority)
                RPC_UpdateUIForEveryone();
            // 서버일 때만 조건 검사
            if (Runner.IsServer)
            {
                Invoke(nameof(TryGameStart), 0.7f);
            }
        }
    }
    private void TryGameStart()
    {
        ServerManager.Instance.roomController.TryStartGameIfReady();
    }
    /// <summary>
    /// 레디 상태를 토글하는 함수. 본인만 호출 가능.
    /// </summary>
    public void ToggleReady()
    {
        if (HasInputAuthority)
        {
            RPC_RequestToggleReady(); // 서버에게 변경 요청만 보냄
        }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestToggleReady()
    {
        IsReady = !IsReady;
        Debug.Log($"[서버] {Nickname} 레디 상태 변경됨: {IsReady}");
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_UpdateUIForEveryone()
    {
        Debug.Log($"[RPC] 모든 클라이언트 UI 갱신 호출");
        ServerManager.Instance.roomController?.UpdateLobbyUI();
    }
}
