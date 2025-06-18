using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem.Switch;

public class PlayerSound : NetworkBehaviour
{
    public enum EPlayerSoundType
    {
        None = 0,
        Attack,
        Hit,
        Heal,
        Flash
    }

    [Header("References")]
    [SerializeField] private AudioClip[] _attackSounds;
    [SerializeField] private AudioClip[] _hitSounds;
    [SerializeField] private AudioClip[] _healSounds;
    [SerializeField] private AudioClip[] _flashSounds;

    //IPlayerContext context;

    AudioSource _audio;

    public AudioSource Audio => _audio;

    public void Init()
    {
        //this.context = context;
        _audio = GetComponent<AudioSource>();
    }

    public void PlayOneShot_Attack()
    {
        PlayOneShot_RPC(EPlayerSoundType.Attack);
    }

    public void PlayOneShot_Hit()
    {
        PlayOneShot_RPC(EPlayerSoundType.Hit);
    }

    public void PlayOneShot_Heal()
    {
        PlayOneShot_RPC(EPlayerSoundType.Heal);
    }
    public void PlayOneShot_Flash()
    {
        PlayOneShot_RPC(EPlayerSoundType.Flash);
    }

    private void PlayOneShot(AudioClip clip)
    {
        SoundManager.instance.PlayOneShot(_audio, clip);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void PlayOneShot_RPC(EPlayerSoundType soundType)
    {
        switch (soundType)
        {
            case EPlayerSoundType.None:
                break;
            case EPlayerSoundType.Attack:
                PlayOneShot(_attackSounds[Random.Range(0, _attackSounds.Length)]);
                break;
            case EPlayerSoundType.Hit:
                PlayOneShot(_hitSounds[Random.Range(0, _hitSounds.Length)]);
                break;
            case EPlayerSoundType.Heal:
                PlayOneShot(_healSounds[Random.Range(0, _healSounds.Length)]);
                break;
            case EPlayerSoundType.Flash:
                PlayOneShot(_flashSounds[Random.Range(0, _flashSounds.Length)]);
                break;
        }
    }
}
