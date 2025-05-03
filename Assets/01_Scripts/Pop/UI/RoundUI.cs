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

        // 초기 상태
        roundImage.anchoredPosition = new Vector2(300f, 0f);
        roundImage.localScale = Vector3.one * 0.3f; // 더 작게 시작
        canvasGroup.alpha = 0f;

        // 등장: 빠르게 커지고 튕기듯 멈춤
        seq.Append(roundImage.DOAnchorPos(new Vector2(130f, 0f), 0.2f).SetEase(Ease.OutCubic));
        seq.Join(roundImage.DOScale(1.1f, 0.4f).SetEase(Ease.OutBack));
        seq.Join(canvasGroup.DOFade(1f, 0.3f));

        // 잠깐 대기
        seq.AppendInterval(0.7f);

        // 퇴장: 더 왼쪽으로, 더 크고, 강하게 사라짐
        seq.Append(roundImage.DOAnchorPos(new Vector2(-300f, 100f), 0.6f).SetEase(Ease.InCubic));
        seq.Join(roundImage.DOScale(15f, 0.5f)); // 더 격하게 커짐
        seq.Join(canvasGroup.DOFade(0f, 0.5f));
    }
}
