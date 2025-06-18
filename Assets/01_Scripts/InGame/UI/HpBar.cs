using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Image hpBarLine;

    public void Init()
    {
        hpBarLine.fillAmount = 1.0f;
    }

    public void UpdateHpBar(float ratio)
    {
        hpBarLine.fillAmount = ratio;
    }
}
