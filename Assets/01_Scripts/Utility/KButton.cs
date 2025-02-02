using DG.Tweening;
using PlayerCharacterControl.State;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KButton : Button
{
    [Header("Scale Animation Settings")]
    [SerializeField] private float scaleUpMultiplier = 1.2f;   // ���콺 �÷��� �� �� ��� Ŀ����
    [SerializeField] private float animationDuration = 0.2f;   // �ִϸ��̼� �ð�
    [SerializeField] private Ease easeType = Ease.OutSine;     // �ִϸ��̼� ��¡ ����
    private Vector3 originalScale;   // �⺻ ������
    private RectTransform rectTransform;

    public EPopupType PopupType = EPopupType.None;
    protected override void Awake()
    {
        base.Awake();
        rectTransform = GetComponent<RectTransform>();
        // ���� ���� �������� ����
        originalScale = rectTransform.localScale;
    }

    /// <summary>
    /// ���콺 Ŀ���� UI ���(��ư)�� ������ ��
    /// </summary>
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        rectTransform.DOScale(originalScale * scaleUpMultiplier, animationDuration)
                     .SetEase(easeType);
    }

    /// <summary>
    /// ���콺 Ŀ���� UI ���(��ư)���� ������ ��
    /// </summary>
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        rectTransform.DOScale(originalScale, animationDuration)
                     .SetEase(easeType);
    }

    /// <summary>
    /// ���� Ŭ���� �Ͼ�� �� (�ʿ信 �°� ���)
    /// </summary>
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        // popup ���� ����
        if(EPopupType.None != PopupType)
        {
            PopManager.instance.OpenPopBtn(PopupType);
        }
    }
}
