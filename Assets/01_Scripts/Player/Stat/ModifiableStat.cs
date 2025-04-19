using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifiableStat
{
    public float baseValue;
    private List<StatModifier> modifiers = new List<StatModifier>();

    public ModifiableStat(float baseValue_)
    {
        baseValue = baseValue_;
    }
    public float GetValue()
    {
        float total = baseValue;

        modifiers.RemoveAll(m => !m.IsActive());

        foreach (var mod in modifiers)
            total += mod.value;

        return total;
    }

    public void AddModifier(float value, float duration)
    {
        modifiers.Add(new StatModifier(value, duration));
    }

    public void ModifyBase(float amount)
    {
        baseValue = Mathf.Max(0, baseValue + amount);
    }

    public void ResetBase(float value)
    {
        baseValue = value;
    }
}
