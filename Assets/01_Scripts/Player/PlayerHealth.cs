using Fusion;
using MoreMountains.Feedbacks;
using Mundo_dodgeball.Player.StateMachine;
using System.Collections;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour, IDamageable
{
    private IPlayerContext context;
    //[SerializeField] MMF_Player damageTestController;
    //private MMF_FloatingText mMF_FloatingText;
    [Networked] private float CurrentHealth { get; set; }


    public bool IsDead => CurrentHealth <= 0.0f;
    public float HealthPercentage => CurrentHealth / context.Stats.GetMaxHealth();
    
    
    public RectTransform testHpBar;

    //private void Start()
    //{
    //    foreach (var feedback in damageTestController.FeedbacksList)
    //    {
    //        if (feedback is MMF_FloatingText ft)
    //        {
    //            mMF_FloatingText = ft;
    //            break;
    //        }
    //    }
    //}

    public void Initialize(IPlayerContext context)
    {
        this.context = context;
        context.Stats.ResetHealth();
        CurrentHealth = context.Stats.GetMaxHealth();
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0.0f) return;

        context.Stats.ModifyCurrentHealth(-damage);
        CurrentHealth = context.Stats.GetCurrentHealth();
    }

    public void Heal(float healAmount)
    {
        if (healAmount <= 0.0f) return;

        context.Stats.ModifyCurrentHealth(healAmount);
        CurrentHealth = context.Stats.GetCurrentHealth();
    }

    public override void Render()
    {
        if (context == null) return;
        if (testHpBar != null)
        {
            testHpBar.localScale = new Vector3(HealthPercentage, 1.0f, 1.0f);
        }
    }

    private void OnGUI()
    {
        if (context == null) return;
        GUI.Label(new Rect(0 + Object.InputAuthority.PlayerId * 200, 10, 300, 100), "Current Health" + CurrentHealth);

        if (GUI.Button(new Rect(0, 200, 300, 100), "Die"))
        {
            //stats.SetCurrentHealth(0.0f);
            context.ChangeState(EPlayerState.Die);
        }
        if (GUI.Button(new Rect(0, 300, 300, 100), "TakeDamage 30"))
        {
            TakeDamage(30);
        }
    }
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(PlayerHealth))]
//public class FloatingTextSpawnerEditor : Editor
//{
//    public float damage;
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        PlayerHealth spawner = (PlayerHealth)target;
//        GUILayout.Space(10);
//        // 🔧 데미지 값 입력 필드
//        damage = EditorGUILayout.FloatField("Damage", damage);
//        EditorGUILayout.LabelField(" 테스트 버튼", EditorStyles.boldLabel);
//        if (GUILayout.Button("Damage Test Button"))
//        {
//            spawner.Damage(damage);
//        }
//    }
//}
//#endif