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

        // 초기 상태 설정
        roundImage.anchoredPosition = new Vector2(300f, 0f); // 오른쪽 바깥에서 시작
        roundImage.localScale = Vector3.one * 0.5f;
        canvasGroup.alpha = 0f;

        // 등장 애니메이션: 300 → 130, 스케일 0.5 → 1.0, 페이드 인
        seq.Append(roundImage.DOAnchorPos(new Vector2(130f, 0f), 0.5f).SetEase(Ease.OutCubic));
        seq.Join(roundImage.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
        seq.Join(canvasGroup.DOFade(1f, 0.5f));

        // x=130에서 0.7초 대기
        seq.AppendInterval(0.7f);

        // 퇴장 애니메이션: 130 → -130, 스케일 1.0 → 1.5, 페이드 아웃
        seq.Append(roundImage.DOAnchorPos(new Vector2(-130f, 0f), 0.5f).SetEase(Ease.InCubic));
        seq.Join(roundImage.DOScale(1.5f, 0.5f));
        seq.Join(canvasGroup.DOFade(0f, 0.5f));
    }

}
