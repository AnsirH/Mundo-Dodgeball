using Fusion;
using NUnit.Framework.Constraints;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct PlayerStatData : INetworkStruct
{
    public float AttackPower;
    public float Health;
    public float MoveSpeed;
    public float HealthRegen;
}

[System.Serializable]
public class PlayerStats
{
    [Header("Primary Stats")]
    public ModifiableStat AttackPower = new(30);
    public ModifiableStat MaxHealth = new(100);
    public ModifiableStat MoveSpeed = new(3);
    public ModifiableStat AttackCooldown = new(3);
    public ModifiableStat HealthRegen = new(0.2f);

    private float _currentHealth;
    private float _regenAccumulator = 0f;

    public PlayerStats()
    {
        _currentHealth = GetMaxHealth();
    }
    public bool HandleHealthRegen(float deltaTime, out float regenedHealth)
    {
        regenedHealth = _currentHealth;
        _regenAccumulator += deltaTime;
        if (_regenAccumulator >= 1f)
        {
            if(!IsDead())
            {
                _regenAccumulator = 0f;
                RegenerateHealth();
                regenedHealth = _currentHealth;
                return true;
            }
        }
        return false;
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
