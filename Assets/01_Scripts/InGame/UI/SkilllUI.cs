using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkilllUI : MonoBehaviour
{
    public Image coolTimeImage;
    public TextMeshProUGUI coolTimeText;

    public void Init()
    {
        coolTimeImage.fillAmount = 0.0f;
        coolTimeText.text = string.Empty;
    }

    public void UpdateCoolTime(float remainingCoolTime, float maxCoolTime)
    {
        if (remainingCoolTime == 0.0f)
        {
            Init();
            return;
        }

        coolTimeImage.fillAmount = remainingCoolTime / maxCoolTime;

        if (remainingCoolTime > 1.0f)
            coolTimeText.text = ((float)Math.Round(remainingCoolTime)).ToString();
        else
            coolTimeText.text = ((float)Math.Round(remainingCoolTime, 1)).ToString();
    }
}
