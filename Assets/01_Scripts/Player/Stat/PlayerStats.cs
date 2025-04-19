using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Primary Stats")]
    public ModifiableStat AttackPower = new ModifiableStat(10f);
    public ModifiableStat MaxHealth = new ModifiableStat(100f);
    public ModifiableStat MoveSpeed = new ModifiableStat(5f);
    public ModifiableStat AttackCooldown = new ModifiableStat(1f);
    public ModifiableStat HealthRegen = new ModifiableStat(1f);

    private float _currentHealth;
    private float _regenAccumulator = 0f;

    private void Awake()
    {
        _currentHealth = MaxHealth.GetValue();
    }

    private void Update()
    {
        HandleHealthRegen();
    }

    private void HandleHealthRegen()
    {
        _regenAccumulator += Time.deltaTime;
        if (_regenAccumulator >= 1f)
        {
            if(!IsDead())
            {
                _regenAccumulator = 0f;
                RegenerateHealth();
            }
        }
    }

    private void RegenerateHealth()
    {
        if (_currentHealth < GetMaxHealth())
            ModifyCurrentHealth(HealthRegen.GetValue());
    }

    // ===== Getters =====
    public float GetAttackPower() => AttackPower.GetValue();
    public float GetMaxHealth() => MaxHealth.GetValue();
    public float GetMoveSpeed() => MoveSpeed.GetValue();
    public float GetAttackCooldown() => AttackCooldown.GetValue();
    public float GetHealthRegen() => HealthRegen.GetValue();
    public float GetCurrentHealth() => _currentHealth;

    // ===== Permanent Modifiers =====
    public void IncreaseAttackPower(float amount) => AttackPower.ModifyBase(amount);
    public void IncreaseMaxHealth(float amount) => MaxHealth.ModifyBase(amount);
    public void IncreaseMoveSpeed(float amount) => MoveSpeed.ModifyBase(amount);
    public void DecreaseAttackCooldown(float amount) => AttackCooldown.ModifyBase(-amount);
    public void IncreaseHealthRegen(float amount) => HealthRegen.ModifyBase(amount);

    // ===== Temporary Buffs =====n    
    public void AddAttackBuff(float amount, float duration) => AttackPower.AddModifier(amount, duration);
    public void AddHealthBuff(float amount, float duration) => MaxHealth.AddModifier(amount, duration);
    public void AddSpeedBuff(float amount, float duration) => MoveSpeed.AddModifier(amount, duration);
    public void AddCooldownDebuff(float amount, float duration) => AttackCooldown.AddModifier(-amount, duration);
    public void AddRegenBuff(float amount, float duration) => HealthRegen.AddModifier(amount, duration);

    // ===== Health Control =====
    public void ModifyCurrentHealth(float delta)
    {
        _currentHealth = Mathf.Clamp(_currentHealth + delta, 0f, GetMaxHealth());
    }
    /// <summary>직접 현재 체력을 설정합니다 (0~MaxHealth 범위로 클램프)</summary>
    public void SetCurrentHealth(float health)
    {
        _currentHealth = Mathf.Clamp(health, 0f, GetMaxHealth());
    }
    public bool IsDead() => _currentHealth <= 0f;

    public void ResetHealth()
    {
        _currentHealth = GetMaxHealth();
    }
}
