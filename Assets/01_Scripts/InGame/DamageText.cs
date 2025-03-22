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
                token.ThrowIfCancellationRequested(); // �߰��� ��ҵǸ� ���⼭ �ߴܵ�

                float t = elapsed / destroyTime;

                // ��ġ �̵�
                myPos.position = startPos + Vector3.up * t * moveSpeed;

                // ���� ���� ���߱�
                Color c = originalColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                text_sample.color = c;

                elapsed += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token); // ��ū ���� �ʼ�
            }

            Destroy(gameObject);
        }
        catch (OperationCanceledException)
        {
            // �ƹ��͵� �� �ص� ��. �����ϰ� �����.
        }
    
    }
}
