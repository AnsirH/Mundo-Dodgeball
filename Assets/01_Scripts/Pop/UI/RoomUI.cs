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
        // �ö󰡾� �Ѵٸ�( true )
        if (_isReady)
        {
            // ���̰� 0�̶�� �����ϰ� targetHeight���� Ʈ��
            targetRect.DOSizeDelta(new Vector2(targetRect.sizeDelta.x, 60), 0.3f)
                      .SetEase(Ease.OutCubic);
        }
        else
        {
            // �������� �Ѵٸ�( false )
            // ���� ���̰� 60�̶�� �����ϰ� 0���� Ʈ��
            targetRect.DOSizeDelta(new Vector2(targetRect.sizeDelta.x, 0f), 0.3f)
                      .SetEase(Ease.InCubic);
        }
    }
}
