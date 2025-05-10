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
    public void ShowResultAnimPlay(bool isWin)
    {
        sprite.sprite = isWin
            ? GameManager.Instance.resourceManager.GetResultImage(0)
            : GameManager.Instance.resourceManager.GetResultImage(1);

        Sequence seq = DOTween.Sequence();

        // 초기 상태: 엄청 크게 중앙에
        roundImage.anchoredPosition = Vector2.zero;
        roundImage.localScale = Vector3.one * 18f;
        canvasGroup.alpha = 0f;

        // 화면에 꽉 차는 크기
        Vector3 targetScale = Vector3.one;

        // 점점 줄어들면서 등장 + 투명도 증가
        seq.Append(canvasGroup.DOFade(1f, 0.2f));
        seq.Join(roundImage.DOScale(targetScale, 0.4f).SetEase(Ease.OutCubic));

        // 충격 효과로 흔들림 (펀치 효과)
        seq.Append(roundImage.DOPunchPosition(new Vector3(10f, 5f, 0f), 0.4f, 10, 1f));
    }
}
