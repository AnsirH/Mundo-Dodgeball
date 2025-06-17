using Fusion;
using System.Collections;
using UnityEngine;
using Mundo_dodgeball.Spell;

public class PlayerSpellActuator : NetworkBehaviour
{
    public void Initialize(IPlayerContext context)
    {
        this.context = context;
    }

    public void ExecuteD(Vector3 targetPoint)
    {
        if (CoolTimerD.ExpiredOrNotRunning(Runner))
        {
            ExecuteSpell(spellD, targetPoint);
            CoolTimerD = TickTimer.CreateFromSeconds(Runner, spellD._maxCoolTime);
        }   
    }

    public void ExecuteF(Vector3 targetPoint)
    {
        if (CoolTimerF.ExpiredOrNotRunning(Runner))
        {
            ExecuteSpell(spellF, targetPoint);
            CoolTimerF = TickTimer.CreateFromSeconds(Runner, spellF._maxCoolTime);
        }
    }

    private void ExecuteSpell(SpellData spellData, Vector3 targetPoint)
    {
        switch (spellData._category)
        {
            case SpellCategory.None:
                break;
            case SpellCategory.Heal:
                context.Health.Heal(spellData._valueAmount);
                break;
            case SpellCategory.Flash:

                Vector3 direction = targetPoint - context.Movement.transform.position;
                Vector3 destination = context.Movement.transform.position;
                if (direction.magnitude > spellData._valueAmount) destination += direction.normalized * spellData._valueAmount;
                else destination += direction;
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
                context.Movement.Teleport(destination);
                context.Movement.transform.rotation = targetRotation;
                break;
        }
    }

    public IPlayerContext context;

    public SpellData spellD;
    public SpellData spellF;

    [Networked] private TickTimer CoolTimerD { get; set; }
    [Networked] private TickTimer CoolTimerF { get; set; }
}

