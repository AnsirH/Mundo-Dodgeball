namespace PlayerCharacterControl.State
{
    /// <summary> ĳ������ ���¸� ��Ÿ���� </summary>
    public enum EPlayerState
    {
        None = -1,
        Idle = 0,
        Move,
        Attack,
        Die
    }
    ///<summary>�˾� ����</summary>
    public enum EPopupType
    {
        None = -1,
        GameSelect = 0,
        LocalSelect = 1,
        SettingPop = 2,
    }
}
