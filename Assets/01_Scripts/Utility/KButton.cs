using DG.Tweening;
using PlayerCharacterControl.State;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KButton : Button
{
    [Header("Scale Animation Settings")]
    [SerializeField] private float scaleUpMultiplier = 1.2f;   // 마우스 올렸을 때 몇 배로 커질지
    [SerializeField] private float animationDuration = 0.2f;   // 애니메이션 시간
    [SerializeField] private Ease easeType = Ease.OutSine;     // 애니메이션 이징 종류
    private Vector3 originalScale;   // 기본 스케일
    private RectTransform rectTransform;

    public EPopupType PopupType = EPopupType.None;
    protected override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
        // 현재 시작 스케일을 저장
        originalScale = rectTransform.localScale;
    }

    /// <summary>
    /// 마우스 커서가 UI 요소(버튼)에 들어왔을 때
    /// </summary>
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        rectTransform.DOScale(originalScale * scaleUpMultiplier, animationDuration)
                     .SetEase(easeType);
    }

    /// <summary>
    /// 마우스 커서가 UI 요소(버튼)에서 나갔을 때
    /// </summary>
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        rectTransform.DOScale(originalScale, animationDuration)
                     .SetEase(easeType);
    }

    /// <summary>
    /// 실제 클릭이 일어났을 때 (필요에 맞게 사용)
    /// </summary>
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        // popup 열기 전용
        if(EPopupType.None != PopupType)
        {
            PopManager.instance.OpenPopBtn(PopupType);
        }
    }
}
