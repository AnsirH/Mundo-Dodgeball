using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalSelectPop : PopBase
{
    [SerializeField] TMP_Dropdown localDropdown;
    string selectLocal;
    public void OnEnable()
    {
        selectLocal = localDropdown.options[0].text;
    }
    public void OnDropDownChangeValue(int index)
    {
        selectLocal = localDropdown.options[index].text;
    }
    public void ClickSelectLocal()
    {
        ServerManager.Instance.ApplyRegionSetting(selectLocal);
        Close();
    }
}
