// NetworkPlayer.cs
using Fusion;
public class NetworkPlayer : NetworkBehaviour
{
    [Networked] public NetworkString<_32> NickName { get; set; }

    public override void Spawned()
    {
        // StateAuthority(ȣ��Ʈ)�� ���� �Ҵ��� �ֹǷ�,
        // ���⼭�� �ƹ��͵� �� �ص� �˴ϴ�.
    }
}
