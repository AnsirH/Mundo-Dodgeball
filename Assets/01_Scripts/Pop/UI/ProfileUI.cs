using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    [SerializeField] TMP_Text userId;
    [SerializeField] Image userIcon;
    void Start()
    {
        userId.text = SteamManager.GetSteamName();
    }
    public void SetIcon(int i)
    {
        userIcon.sprite = GameManager.Instance.resourceManager.GetPlayerIcon(i);
    }
}
