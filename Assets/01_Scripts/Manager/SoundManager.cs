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

    [Header("����� �ҽ�")]
    public AudioSource bgmSource;
    public AudioSource ingameBgmSource;
    public AudioClip inGameBGM_1;
    public AudioClip inGameBGM_2;
    public AudioClip lobbyBGM;

    public AudioSource uiSfxSource;
    public AudioClip buttonClickClip;
    // dB �ּ�/�ִ� (���� 0~1 �� dB ��ȯ ��)
    private const float MIN_DB = -2000f;
    private const float MAX_DB = -20f;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += PlayOutGameBGM;
    }
    // �κ�/�޴� (OutGame) ���� ����
    public void SetOutGameVolume(float volume)
    {
        bgmSource.volume = volume;
    }
    // �ΰ��� (InGame) ���� ����
    public void SetInGameVolume(float volume)
    {
        ingameBgmSource.volume = volume;
    }

    // ��ư�� ������ �Լ�
    public void OnClickButton()
    {
        // ��ư Ŭ�� ����
        if (uiSfxSource != null && buttonClickClip != null)
        {
            uiSfxSource.PlayOneShot(buttonClickClip);
        }
    }

    // out BGM ����
    public void PlayOutGameBGM(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            // �κ� BGM ���
            if (bgmSource != null && lobbyBGM != null)
            {
                bgmSource.clip = lobbyBGM;
                bgmSource.loop = true;
                bgmSource.Play();
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            int result = Random.Range(0, 2); // 0 �Ǵ� 1
            // �κ� BGM ���
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
