using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCreatePop : MonoBehaviour
{
    private void OnEnable()
    {
        PopManager.instance.gameSelectPop.ButtonSwitch(false);
    }
    private void OnDisable()
    {
        PopManager.instance.gameSelectPop.ButtonSwitch(true);
    }
}
