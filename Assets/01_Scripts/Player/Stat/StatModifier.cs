using UnityEngine;

public class StatModifier
{
    public float value;
    public float startTime;
    public float duration;

    public StatModifier(float value, float duration)
    {
        this.value = value;
        this.duration = duration;
        this.startTime = Time.time;
    }

    public bool IsActive()
    {
        return Time.time <= startTime + duration;
    }
}
