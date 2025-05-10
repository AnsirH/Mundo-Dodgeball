using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InGameResultUI : MonoBehaviour
{
    public Image sprite;
    public RectTransform roundImage;
    public CanvasGroup canvasGroup;

    public void RoundAnimPlay(int ind)
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
    public void ShowResultAnimPlay(bool isWin)
    {
        sprite.sprite = isWin
            ? GameManager.Instance.resourceManager.GetResultImage(0)
            : GameManager.Instance.resourceManager.GetResultImage(1);

        Sequence seq = DOTween.Sequence();

        // �ʱ� ����: ��û ũ�� �߾ӿ�
        roundImage.anchoredPosition = Vector2.zero;
        roundImage.localScale = Vector3.one * 18f;
        canvasGroup.alpha = 0f;

        // ȭ�鿡 �� ���� ũ��
        Vector3 targetScale = Vector3.one;

        // ���� �پ��鼭 ���� + ���� ����
        seq.Append(canvasGroup.DOFade(1f, 0.2f));
        seq.Join(roundImage.DOScale(targetScale, 0.4f).SetEase(Ease.OutCubic));

        // ��� ȿ���� ��鸲 (��ġ ȿ��)
        seq.Append(roundImage.DOPunchPosition(new Vector3(10f, 5f, 0f), 0.4f, 10, 1f));
    }
}
