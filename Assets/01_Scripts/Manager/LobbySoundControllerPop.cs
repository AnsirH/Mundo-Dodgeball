using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySoundControllerPop : PopBase
{
    [Header("OutGame 볼륨 슬라이더")]
    [SerializeField] private Slider outGameVolumeSlider;
    [SerializeField] private Slider inGameVolumeSlider;
    // Start is called before the first frame update
    void Start()
    {
        // 로비 볼륨 슬라이더 설정
        if (outGameVolumeSlider != null)
        {
            // 슬라이더 값 변경 시 AudioManager로 전달
            outGameVolumeSlider.onValueChanged.AddListener(OnOutGameVolumeChanged);

            // 처음 값 세팅
            outGameVolumeSlider.value = 1.0f;
        }
        // 로비 볼륨 슬라이더 설정
        if (inGameVolumeSlider != null)
        {
            // 슬라이더 값 변경 시 AudioManager로 전달
            inGameVolumeSlider.onValueChanged.AddListener(OnInGameVolumeChanged);

            // 처음 값 세팅
            inGameVolumeSlider.value = 1.0f;
        }
    }
    private void OnOutGameVolumeChanged(float value)
    {
        SoundManager.instance.SetOutGameVolume(value);
    }
    private void OnInGameVolumeChanged(float value)
    {
        SoundManager.instance.SetInGameVolume(value);
    }
}
