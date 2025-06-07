// NetworkPlayer.cs
using Fusion;
public class NetworkPlayer : NetworkBehaviour
{
    [Networked] public NetworkString<_32> NickName { get; set; }

    public override void Spawned()
    {
        // StateAuthority(호스트)가 직접 할당해 주므로,
        // 여기서는 아무것도 안 해도 됩니다.
    }
}
