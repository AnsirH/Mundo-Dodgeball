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
    public AudioSource ingameBgmSource;
    public AudioClip inGameBGM_1;
    public AudioClip inGameBGM_2;
    public AudioClip lobbyBGM;

    public AudioSource uiSfxSource;
    public AudioClip buttonClickClip;
    // dB 최소/최대 (볼륨 0~1 → dB 변환 시)
    private const float MIN_DB = -2000f;
    private const float MAX_DB = -20f;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += PlayOutGameBGM;
    }
    // 로비/메뉴 (OutGame) 볼륨 조절
    public void SetOutGameVolume(float volume)
    {
        bgmSource.volume = volume;
    }
    // 인게임 (InGame) 볼륨 조절
    public void SetInGameVolume(float volume)
    {
        ingameBgmSource.volume = volume;
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
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            // 로비 BGM 재생
            if (bgmSource != null && lobbyBGM != null)
            {
                bgmSource.clip = lobbyBGM;
                bgmSource.loop = true;
                bgmSource.Play();
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            int result = Random.Range(0, 2); // 0 또는 1
            // 로비 BGM 재생
            if (inGameBGM_1 != null && result == 0)
            {
                ingameBgmSource.clip = inGameBGM_1;
            }
            else if (ingameBgmSource != null && result == 1)
            {
                ingameBgmSource.clip = inGameBGM_2;
            }
            ingameBgmSource.loop = true;
            ingameBgmSource.Play();
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
