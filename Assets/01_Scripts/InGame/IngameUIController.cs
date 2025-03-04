using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class IngameUIController : MonoBehaviour
{
    private void Update()
    {
        if (isInitialized)
        {
            foreach (HpBar hpBar in HpBars)
            {
                hpBar.UpdateLocate();
                hpBar.UpdateDisplay();
            }
        }
    }

    public void Init()
    {
        int playerCount = IngameController.Instance.playerControllers.Length;
        HpBars = new HpBar[playerCount];
        for (int i = 0; i < playerCount; ++i)
        {
            HpBars[i] = Instantiate(hpBarPrefab, transform).GetComponent<HpBar>();
            HpBars[i].Init(IngameController.Instance.playerControllers[i].Health);
        }

        isInitialized = true;
    }

    public HpBar[] HpBars { get; private set; }
    public GameObject hpBarPrefab;

    public bool isInitialized = false;
}
