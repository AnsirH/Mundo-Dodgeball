using UnityEngine;

public class Heal : Spell
{
    public Heal(IPlayerContext context) : base(context)
    {
        this.context = context;
    }

    public override void Execute()
    {
        context.Stats.ModifyCurrentHealth(healAmount);

        currentCoolTime = maxCoolTime;
    }

    float healAmount = 60.0f;
}
