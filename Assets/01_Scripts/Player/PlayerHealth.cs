using Fusion;
using Mundo_dodgeball.Player.StateMachine;
using System.Collections;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour, IDamageable
{
    private IPlayerContext context;
    [Networked] public float CurrentHealth { get; set; }
    [Networked] public PlayerRef Killer { get; set; }
    public bool IsDead => CurrentHealth <= 0.0f;
    public float HealthPercentage => CurrentHealth / context.Stats.GetMaxHealth();

    public void Initialize(IPlayerContext context)
    {
        this.context = context;
        context.Stats.ResetHealth();
        CurrentHealth = context.Stats.GetMaxHealth();
    }

    public void TakeDamage(float damage, PlayerRef sender)
    {
        if (damage <= 0.0f) return;

        context.Stats.ModifyCurrentHealth(-damage);
        CurrentHealth = context.Stats.GetCurrentHealth();
        context.Sound.PlayOneShot_Hit();
        if (IsDead)
        {
            Killer = sender;
            context.ChangeState(EPlayerState.Die);
        }
    }

    public void Heal(float healAmount)
    {
        if (healAmount <= 0.0f) return;

        context.Stats.ModifyCurrentHealth(healAmount);
        CurrentHealth = context.Stats.GetCurrentHealth();
    }

    public override void FixedUpdateNetwork()
    {
        if (IsDead) return;
        if (context.CurrentState is not PlayerDieState && CurrentHealth <= 0.0f)
        {
            context.ChangeState(EPlayerState.Die);
        }
        context.Stats.SetCurrentHealth(CurrentHealth);
        if (context.Stats.HandleHealthRegen(Runner.DeltaTime, out float regendHealth))
            CurrentHealth = regendHealth;
        //CurrentHealth = context.Stats.GetCurrentHealth();
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