namespace PlayerCharacterControl.State
{
    /// <summary> 캐릭터의 상태를 나타낸다 </summary>
    public enum EPlayerState
    {
        None = -1,
        Idle = 0,
        Move,
        Attack,
        Die
    }
    ///<summary>팝업 종류</summary>
    public enum EPopupType
    {
        None = -1,
        GameSelect = 0,
        LocalSelect = 1,
        SettingPop = 2,
    }
}
