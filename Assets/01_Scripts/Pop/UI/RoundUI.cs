using DG.Tweening;
using UnityEngine;

public class RoundUI : MonoBehaviour
{
    public Sprite sprite;
    public RectTransform roundImage;
    public CanvasGroup canvasGroup;

    public void PlayAnimation(int ind)
    {
        sprite = GameManager.Instance.resourceManager.GetRoundImage(ind); 
        Sequence seq = DOTween.Sequence();

        // �ʱ� ���� ����
        roundImage.anchoredPosition = new Vector2(300f, 0f); // ������ �ٱ����� ����
        roundImage.localScale = Vector3.one * 0.5f;
        canvasGroup.alpha = 0f;

        // ���� �ִϸ��̼�: 300 �� 130, ������ 0.5 �� 1.0, ���̵� ��
        seq.Append(roundImage.DOAnchorPos(new Vector2(130f, 0f), 0.5f).SetEase(Ease.OutCubic));
        seq.Join(roundImage.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
        seq.Join(canvasGroup.DOFade(1f, 0.5f));

        // x=130���� 0.7�� ���
        seq.AppendInterval(0.7f);

        // ���� �ִϸ��̼�: 130 �� -130, ������ 1.0 �� 1.5, ���̵� �ƿ�
        seq.Append(roundImage.DOAnchorPos(new Vector2(-130f, 0f), 0.5f).SetEase(Ease.InCubic));
        seq.Join(roundImage.DOScale(1.5f, 0.5f));
        seq.Join(canvasGroup.DOFade(0f, 0.5f));
    }

}
