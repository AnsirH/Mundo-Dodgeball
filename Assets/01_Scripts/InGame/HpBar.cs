using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    /// <summary> 체력바를 플레이어에게 매치합니다.( 체력바 초기화, 체력바 타겟 설정 ) </summary>
    /// <param name="targetPlayerHealth">할당할 플레이어의 체력 컴포넌트</param>
    public void Init(PlayerHealth targetPlayerHealth)
    {
        playerHealth = targetPlayerHealth;
        targetTrf = targetPlayerHealth.HpBarTrf;
    }
    /// <summary> 체력값 반영 </summary>
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

    /// <summary> 체력바 위치 업데이트 </summary>
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
