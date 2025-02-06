using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomUI : MonoBehaviour
{
    [SerializeField] GameObject noneReadyBtn;
    [SerializeField] GameObject readyBtn;
    public RectTransform masterRect;
    public RectTransform hostRect;
    public TMP_Text leftPlayerText;
    public TMP_Text rightPlayerText;

    private bool isReady = false;
    public void RoomExit()
    {
        ServerManager.Instance.roomManager.LeaveRoom();
    }
    public void ClickReady()
    {
        isReady = !isReady;
        noneReadyBtn.gameObject.SetActive(!isReady);
        readyBtn.gameObject.SetActive(isReady);
        ServerManager.Instance.roomManager.OnClickReady(isReady);
    }
    public void SetImReady(bool _isMaster, bool _isReady)
    {
        RectTransform targetRect = _isMaster ? masterRect : hostRect;
        // 올라가야 한다면( true )
        if (_isReady)
        {
            // 높이가 0이라고 가정하고 targetHeight까지 트윈
            targetRect.DOSizeDelta(new Vector2(targetRect.sizeDelta.x, 60), 0.3f)
                      .SetEase(Ease.OutCubic);
        }
        else
        {
            // 내려가야 한다면( false )
            // 현재 높이가 60이라고 가정하고 0까지 트윈
            targetRect.DOSizeDelta(new Vector2(targetRect.sizeDelta.x, 0f), 0.3f)
                      .SetEase(Ease.InCubic);
        }
    }
}
