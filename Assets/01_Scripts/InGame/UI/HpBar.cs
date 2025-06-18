using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Image hpBarLine;
    public RectTransform RectTransform { get; private set; }

    public void Init()
    {
        hpBarLine.fillAmount = 1.0f;
        RectTransform = GetComponent<RectTransform>();
    }

    public void UpdateHpBar(float ratio)
    {
        hpBarLine.fillAmount = ratio;
    }
}
