using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : ManagerBase<SoundManager>
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("오디오 소스")]
    public AudioSource bgmSource;
    public AudioClip lobbyBGM;

    public AudioSource uiSfxSource;
    public AudioClip buttonClickClip;
    // dB 최소/최대 (볼륨 0~1 → dB 변환 시)
    private const float MIN_DB = -80f;
    private const float MAX_DB = 0f;
    
    // 로비/메뉴 (OutGame) 볼륨 조절
    public void SetOutGameVolume(float volume)
    {
        float dB = Mathf.Lerp(MIN_DB, MAX_DB, volume);
        audioMixer.SetFloat("OutGameVolume", dB);
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
}
