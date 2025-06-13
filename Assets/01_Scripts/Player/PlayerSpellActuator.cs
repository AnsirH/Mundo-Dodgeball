using Fusion;
using System.Collections;
using UnityEngine;
using Mundo_dodgeball.Spell;

public class PlayerSpellActuator : NetworkBehaviour, IPlayerComponent
{
    public void Initialize(IPlayerContext context, bool isOfflineMode = false)
    {
        this.context = context;
        this.isOfflineMode = isOfflineMode;
    }

    public void OnEnabled()
    {
    }

    public void OnDisabled()
    {
    }

    public void HandleInput(NetworkInputData data)
    {
        if (data.buttons.IsSet(NetworkInputData.BUTTOND))
        {
            if (HasStateAuthority && delayD.ExpiredOrNotRunning(Runner))
            {
                delayD = TickTimer.CreateFromSeconds(Runner, spellD._maxCoolTime);
                ExecuteSpell(spellD, data);
            }
        }

        if (data.buttons.IsSet(NetworkInputData.BUTTONF))
        {
            if (HasStateAuthority && delayF.ExpiredOrNotRunning(Runner))
            {
                delayF = TickTimer.CreateFromSeconds(Runner, spellF._maxCoolTime);
                ExecuteSpell(spellF, data);
            }
        }
    }

    private void ExecuteSpell(SpellData spellData, NetworkInputData inputData)
    {
        switch (spellData._category)
        {
            case SpellCategory.None:
                break;
            //case SpellCategory.Heal:
            //    context.Stats.ModifyCurrentHealth(spellData._valueAmount);
            //    RPC_SpawnEffect("HealEffect", context.NCC.transform.position);
            //    break;
            //case SpellCategory.Flash:
            //    Vector3 direction = inputData.targetPoint - context.NCC.transform.position;
            //    float distance = direction.magnitude;
            //    if (distance > spellData._valueAmount)
            //    {
            //        distance = spellData._valueAmount;
            //        direction = direction.normalized * distance;
            //    }
            //    Quaternion targetRotation = Quaternion.LookRotation(inputData.targetPoint - context.NCC.transform.position);
            //    context.NCC.Teleport(context.NCC.transform.position + direction, targetRotation);
            //    RPC_SpawnEffect("FlashEffect", context.NCC.transform.position);
            //    break;
        }
    }

    private IEnumerator SpawnEffect(string effectTag, Vector3 targetPoint, float duration = 1.0f, bool isChild=false)
    {
        GameObject effect = ObjectPooler.GetLocal(effectTag);

        if (effect == null)
        {
            Debug.LogError($"Effect with tag {effectTag} not found in ObjectPooler.");
            yield break;
        }

        effect.transform.position = targetPoint;
        if (isChild)
            effect.transform.parent = context.Movement.transform;

        yield return new WaitForSeconds(duration);

        ObjectPooler.ReleaseLocal(effectTag, effect);
    }

    [Rpc]
    private void RPC_SpawnEffect(string effectTag, Vector3 targetPoint, float duration = 1.0f, bool isChild = false)
    {
        StartCoroutine(SpawnEffect(effectTag, targetPoint, duration, isChild));
    }

    public IPlayerContext context;
    private bool isOfflineMode;

    public SpellData spellD;
    public SpellData spellF;

    public bool Controllable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    [Networked] private TickTimer delayD { get; set; }
    [Networked] private TickTimer delayF { get; set; }
}

