using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    /// <summary> ü�¹ٸ� �÷��̾�� ��ġ�մϴ�.( ü�¹� �ʱ�ȭ, ü�¹� Ÿ�� ���� ) </summary>
    /// <param name="targetPlayerHealth">�Ҵ��� �÷��̾��� ü�� ������Ʈ</param>
    public void Init(PlayerHealth targetPlayerHealth)
    {
        playerHealth = targetPlayerHealth;
        targetTrf = targetPlayerHealth.HpBarTrf;
    }
    /// <summary> ü�°� �ݿ� </summary>
    public void UpdateDisplay()
    {
        if (playerHealth == null)
        {
            Debug.Log($"{gameObject.name} is not matched PlayerHealth");
            return;
        }

        float resultHp = 0;

        if (playerHealth.health >= 0 && playerHealth.maxHealth > 0) 
            resultHp = playerHealth.health / playerHealth.maxHealth;

        valueBarRtrf.localScale = new Vector3(resultHp, 1.0f, 1.0f);
    }

    /// <summary> ü�¹� ��ġ ������Ʈ </summary>
    public void UpdateLocate()
    {
        if (targetTrf == null)
        {
            Debug.LogError("hp bar is not matched target");
            return;
        }

        Vector2 targetScreenPos = Camera.main.WorldToScreenPoint(targetTrf.transform.position);
        rectTransform.position = targetScreenPos;
    }

    public PlayerHealth playerHealth;
    public RectTransform rectTransform;
    public RectTransform valueBarRtrf;
    public Transform targetTrf;
}
