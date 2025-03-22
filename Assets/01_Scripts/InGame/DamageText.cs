using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;
using System.Threading;
using System;
using static UnityEditor.PlayerSettings;
public class DamageText : MonoBehaviour
{
    public float moveSpeed;
    public float alphaSpeed;
    public float destroyTime;
    public TextMeshPro text_sample;
    private Color originalColor;
    private Transform myPos;
    private void Start()
    {
        originalColor = text_sample.color;
    }
    public void OnDamageText(Transform pos, int damage)
    {
        text_sample.text = damage.ToString();
        myPos = pos;
        StartDamageFx(this.GetCancellationTokenOnDestroy()).Forget();
    }
    private async UniTaskVoid StartDamageFx(CancellationToken token)
    {
        float elapsed = 0f;
        Vector3 startPos = myPos.position;
        try
        {
            while (elapsed < destroyTime)
            {
                token.ThrowIfCancellationRequested(); // 중간에 취소되면 여기서 중단됨

                float t = elapsed / destroyTime;

                // 위치 이동
                myPos.position = startPos + Vector3.up * t * moveSpeed;

                // 투명도 점점 낮추기
                Color c = originalColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                text_sample.color = c;

                elapsed += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token); // 토큰 전달 필수
            }

            Destroy(gameObject);
        }
        catch (OperationCanceledException)
        {
            // 아무것도 안 해도 됨. 안전하게 종료됨.
        }
    
    }
}
