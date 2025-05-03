using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RoundUI : MonoBehaviour
{
    public Image sprite;
    public RectTransform roundImage;
    public CanvasGroup canvasGroup;

    public void PlayAnimation(int ind)
    {
        sprite.sprite = GameManager.Instance.resourceManager.GetRoundImage(ind);
        Sequence seq = DOTween.Sequence();

        // �ʱ� ����
        roundImage.anchoredPosition = new Vector2(300f, 0f);
        roundImage.localScale = Vector3.one * 0.3f; // �� �۰� ����
        canvasGroup.alpha = 0f;

        // ����: ������ Ŀ���� ƨ��� ����
        seq.Append(roundImage.DOAnchorPos(new Vector2(130f, 0f), 0.2f).SetEase(Ease.OutCubic));
        seq.Join(roundImage.DOScale(1.1f, 0.4f).SetEase(Ease.OutBack));
        seq.Join(canvasGroup.DOFade(1f, 0.3f));

        // ��� ���
        seq.AppendInterval(0.7f);

        // ����: �� ��������, �� ũ��, ���ϰ� �����
        seq.Append(roundImage.DOAnchorPos(new Vector2(-300f, 100f), 0.6f).SetEase(Ease.InCubic));
        seq.Join(roundImage.DOScale(15f, 0.5f)); // �� ���ϰ� Ŀ��
        seq.Join(canvasGroup.DOFade(0f, 0.5f));
    }
}
