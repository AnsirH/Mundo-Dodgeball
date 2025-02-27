using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySoundControllerPop : PopBase
{
    [Header("OutGame ���� �����̴�")]
    [SerializeField] private Slider outGameVolumeSlider;
    [SerializeField] private Slider inGameVolumeSlider;
    // Start is called before the first frame update
    void Start()
    {
        // �κ� ���� �����̴� ����
        if (outGameVolumeSlider != null)
        {
            // �����̴� �� ���� �� AudioManager�� ����
            outGameVolumeSlider.onValueChanged.AddListener(OnOutGameVolumeChanged);

            // ó�� �� ����
            outGameVolumeSlider.value = 1.0f;
        }
        // �κ� ���� �����̴� ����
        if (inGameVolumeSlider != null)
        {
            // �����̴� �� ���� �� AudioManager�� ����
            inGameVolumeSlider.onValueChanged.AddListener(OnInGameVolumeChanged);

            // ó�� �� ����
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
