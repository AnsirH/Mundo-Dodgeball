using Fusion;
using System.Collections;
using UnityEngine;
using Mundo_dodgeball.Spell;
using Mundo_dodgeball.Player.StateMachine;

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
                if (Object.HasStateAuthority)
                {
                    Runner.Spawn(spellData._effectPrefab, context.Movement.transform.position);
                    context.Sound.PlayOneShot_Heal();
                }
                break;
            case SpellCategory.Flash:

                if (IngameController.Instance != null && IngameController.Instance.Ground != null)
                {
                    if (IngameController.Instance.Ground.GetAdjustedPoint(Object.InputAuthority.PlayerId - 1, context.Movement.transform.position, targetPoint, out Vector3 adjustedPoint))
                        targetPoint = adjustedPoint;
                } 

                Vector3 direction = targetPoint - context.Movement.transform.position;
                Vector3 destination = context.Movement.transform.position;
                if (direction.magnitude > spellData._valueAmount) destination += direction.normalized * spellData._valueAmount;
                else destination += direction;
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
                context.Movement.Teleport(destination);
                context.Movement.SetRotation(targetRotation);

                context.ChangeState(EPlayerState.Idle);

                if (Object.HasStateAuthority)
                {
                    Runner.Spawn(spellData._effectPrefab, context.Movement.transform.position);
                    context.Sound.PlayOneShot_Flash();
                }
                break;
        }
    }

    public void ResetCoolTime()
    {
        if (CoolTimerD.IsRunning)
            CoolTimerD = TickTimer.None;

        if (CoolTimerF.IsRunning)
            CoolTimerF = TickTimer.None;
    }

    public IPlayerContext context;

    public SpellData spellD;
    public SpellData spellF;

    [Networked] private TickTimer CoolTimerD { get; set; }
    [Networked] private TickTimer CoolTimerF { get; set; }

    public float CoolTimeD => CoolTimerD.RemainingTime(Runner).HasValue ? CoolTimerD.RemainingTime(Runner).Value : 0.0f;
    public float CoolTimeF => CoolTimerF.RemainingTime(Runner).HasValue ? CoolTimerF.RemainingTime(Runner).Value : 0.0f;
}

