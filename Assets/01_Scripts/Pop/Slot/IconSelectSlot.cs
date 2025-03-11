using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconSelectSlot : MonoBehaviour
{
    public Image icon;
    private int index;
    private void init(int i)
    {
        index = i;
        icon.sprite = GameManager.Instance.resourceManager.GetPlayerIcon(index);
    }
    public void SelcetIcon()
    {

    }

}
