using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : ManagerBase<SoundManager>
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Group")]
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup bgmGroup;

    [Header("오디오 소스")]
    public AudioSource bgmSource;
    public AudioClip lobbyBGM;

    public AudioSource uiSfxSource;
    public AudioClip buttonClickClip;
    // dB 최소/최대 (볼륨 0~1 → dB 변환 시)
    private const float MIN_DB = -80f;
    private const float MAX_DB = -20f;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += PlayOutGameBGM;
    }
    // 로비/메뉴 (OutGame) 볼륨 조절
    public void SetOutGameVolume(float volume)
    {
        float dB = Mathf.Lerp(MIN_DB, MAX_DB, volume);
        audioMixer.SetFloat("OutGameVolume", dB);
        audioMixer.FindMatchingGroups("Master/OutGameGroup");
    }
    // 인게임 (InGame) 볼륨 조절
    public void SetInGameVolume(float volume)
    {
        float dB = Mathf.Lerp(MIN_DB, MAX_DB, volume);
        audioMixer.SetFloat("InGameVolume", dB);
    }

    // 버튼에 연결할 함수
    public void OnClickButton()
    {
        // 버튼 클릭 사운드
        if (uiSfxSource != null && buttonClickClip != null)
        {
            uiSfxSource.PlayOneShot(buttonClickClip);
        }
    }

    // out BGM 시작
    public void PlayOutGameBGM(Scene scene, LoadSceneMode mode)
    {
        // 로비 BGM 재생
        if (bgmSource != null && lobbyBGM != null)
        {
            bgmSource.clip = lobbyBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void SetPlayerAudioGroup(List<IPlayerContext> playerContexts)
    {
        foreach (var playerContent in playerContexts)
        {
            AddSourceToGroup(playerContent.Sound.Audio, sfxGroup);
        }
    }

    public void AddSourceToGroup(AudioSource source, AudioMixerGroup group)
    {
        source.outputAudioMixerGroup = group;
    }

    public void PlayOneShot(AudioSource source, AudioClip clip)
    {
        if (source.outputAudioMixerGroup == null) source.outputAudioMixerGroup = sfxGroup;
        source.PlayOneShot(clip);
    }
}
