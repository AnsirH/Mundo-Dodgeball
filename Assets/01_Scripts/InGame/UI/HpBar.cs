using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    public RectTransform hpBarRect;

    public void Init()
    {
        hpBarRect.localScale = Vector3.one;
    }

    public void UpdateHpBar(float ratio)
    {
        hpBarRect.localScale = new Vector3(ratio, 1, 1);
    }
}
