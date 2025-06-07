using UnityEngine;

public class InGameSettingPop : PopBase
{
    [SerializeField] GameObject SureGiveUpMessage;
    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        base.Close();
    }
    public void GiveUpClick()
    {
        DetailOpen(SureGiveUpMessage);
    }
    public void CloseGiveUp()
    {
        DetailClose(SureGiveUpMessage);
    }
    // 항복하고 게임 나가기
    public void SureGiveUp()
    {
        Close();
        ServerManager.Instance.roomManager.LeaveRoom();
        
    }
}
