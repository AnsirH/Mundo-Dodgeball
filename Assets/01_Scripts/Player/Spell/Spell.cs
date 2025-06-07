using UnityEngine;
public abstract class Spell
{
    public IPlayerContext context;
    public Spell(IPlayerContext context)
    {
        this.context = context;
    }

    public abstract void Execute();

    public void CoolDown(float decreaseValue)
    {
        if (currentCoolTime > 0.0f)
        {
            currentCoolTime -= decreaseValue;
        }
        else
        {
            currentCoolTime = 0.0f;
        }
    }

    public float currentCoolTime = 0.0f;
    public float maxCoolTime = 5.0f;

    public bool CanUsable { get { return currentCoolTime <= 0.0f; } }
}

