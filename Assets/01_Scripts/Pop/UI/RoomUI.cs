using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    [SerializeField] GameObject noneReadyBtn;
    [SerializeField] GameObject readyBtn;
    public RectTransform masterRect;
    public RectTransform hostRect;
    public TMP_Text leftPlayerText;
    public TMP_Text rightPlayerText;
    public Image leftPlayerImage;
    public Image rightPlayerImage;

    public event Action OnLeaveRoomRequested;
    private bool isReady = false;
    public void RoomExit()
    {
        OnLeaveRoomRequested?.Invoke();
    }
    //public void ClickReady()
    //{
    //    var player = ServerManager.Instance.roomController.FindMyPlayerInfo();
    //    player.ToggleReady();
    //    noneReadyBtn.gameObject.SetActive(!player.IsReady);
    //    readyBtn.gameObject.SetActive(player.IsReady);
    //}
    public async void ClickReady()
    {
        var player = ServerManager.Instance.roomController.FindMyPlayerInfo();
        bool before = player.IsReady;

        player.ToggleReady(); // 서버에 변경 요청

        await UniTask.WaitUntil(() => player.IsReady != before); // ✅ 값 바뀔 때까지 대기

        noneReadyBtn.gameObject.SetActive(!player.IsReady);
        readyBtn.gameObject.SetActive(player.IsReady);
    }
    public void OffReayToggle()
    {
        noneReadyBtn.gameObject.SetActive(true);
        readyBtn.gameObject.SetActive(false);
    }
    public void OnEnable()
    {
        initReady();
    }
    public void initReady()
    {
        masterRect.sizeDelta = new Vector2(masterRect.sizeDelta.x, 0f);
        hostRect.sizeDelta = new Vector2(hostRect.sizeDelta.x, 0f);
    }
    public void SetImReady(bool _isMaster, bool _isReady)
    {
        RectTransform targetRect = _isMaster ? masterRect : hostRect;
        float currentHeight = targetRect.sizeDelta.y;
        float targetHeight = _isReady ? 60f : 0f;

        // 현재 높이와 목표 높이가 거의 같으면 트윈 생략
        if (Mathf.Approximately(currentHeight, targetHeight))
        {
            return;
        }

        // 트윈 실행
        targetRect.DOSizeDelta(new Vector2(targetRect.sizeDelta.x, targetHeight), 0.3f)
                  .SetEase(_isReady ? Ease.OutCubic : Ease.InCubic);
    }


}
