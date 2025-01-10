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
}
