using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : ManagerBase<SoundManager>
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("����� �ҽ�")]
    public AudioSource bgmSource;
    public AudioClip lobbyBGM;

    public AudioSource uiSfxSource;
    public AudioClip buttonClickClip;
    // dB �ּ�/�ִ� (���� 0~1 �� dB ��ȯ ��)
    private const float MIN_DB = -80f;
    private const float MAX_DB = -20f;

    private void Awake()
    {
        SceneManager.sceneLoaded += PlayOutGameBGM;
    }
    // �κ�/�޴� (OutGame) ���� ����
    public void SetOutGameVolume(float volume)
    {
        float dB = Mathf.Lerp(MIN_DB, MAX_DB, volume);
        audioMixer.SetFloat("OutGameVolume", dB);
    }
    // �ΰ��� (InGame) ���� ����
    public void SetInGameVolume(float volume)
    {
        float dB = Mathf.Lerp(MIN_DB, MAX_DB, volume);
        audioMixer.SetFloat("InGameVolume", dB);
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
        // �κ� BGM ���
        if (bgmSource != null && lobbyBGM != null)
        {
            bgmSource.clip = lobbyBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }
}
