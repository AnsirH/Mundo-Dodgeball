using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpell : NetworkBehaviour, IPlayerComponent
{
    private void Awake()
    {
        spellD = new Heal(context);
        spellF = new Flash(context);
    }

    public void Initialize(IPlayerContext context, bool isOfflineMode = false)
    {
        this.context = context;
        this.isOfflineMode = isOfflineMode;
    }

    public void Updated()
    {
        if (spellD.currentCoolTime > 0)
        {
            spellD.CoolDown(Time.deltaTime);
        }

        if (spellF.currentCoolTime > 0)
        {
            spellF.CoolDown(Time.deltaTime);
        }
    }

    public void OnEnabled()
    {
    }

    public void OnDisabled()
    {
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        if (context.action.name == "D")
        {
            if (spellD.CanUsable)
            {
                spellD.Execute();
                StartCoroutine(SpawnEffect("HealEffect", this.context.Trf.position, 1.5f, true));
                //this.context.p_PhotonView.RPC(nameof(SpawnEffect_RPC), RpcTarget.Others, "HealEffect", this.context.Trf.position, 1.5f, true);
            }
        }
        if (context.action.name == "F")
        {
            if (spellF.CanUsable)
            {
                spellF.Execute();
                Vector3 targetPoint = this.context.MousePositionGetter.ClickPoint.Value;
                targetPoint.y = IngameController.Instance.ground.transform.position.y;
                StartCoroutine(SpawnEffect("FlashEffect", targetPoint));
                //this.context.p_PhotonView.RPC(nameof(SpawnEffect_RPC), RpcTarget.Others, "FlashEffect", targetPoint, 1.0f, false);
            }
        }
    }

    public void HandleInput(NetworkInputData data)
    {
        if (data.buttons.IsSet(NetworkInputData.BUTTOND))
        {
            if (HasStateAuthority && delayD.ExpiredOrNotRunning(Runner))
            {
                spellD.Execute();
                delayD = TickTimer.CreateFromSeconds(Runner, spellD.maxCoolTime);
            }
        }

        if (data.buttons.IsSet(NetworkInputData.BUTTONF))
        {
            if (HasStateAuthority && delayF.ExpiredOrNotRunning(Runner))
            {
                spellF.Execute();
                delayF = TickTimer.CreateFromSeconds(Runner, spellF.maxCoolTime);
            }
        }
    }

    private IEnumerator SpawnEffect(string effectTag, Vector3 targetPoint, float duration = 1.0f, bool isChild=false)
    {
        GameObject effect = ObjectPooler.Get(effectTag);

        if (effect == null)
        {
            Debug.LogError($"Effect with tag {effectTag} not found in ObjectPooler.");
            yield break;
        }

        effect.transform.position = targetPoint;
        if (isChild)
            effect.transform.parent = context.Trf;

        yield return new WaitForSeconds(duration);

        ObjectPooler.Release(effectTag, effect);
    }

    [Rpc]
    private void RPC_SpawnEffect(string effectTag, Vector3 targetPoint, float duration = 1.0f, bool isChild = false)
    {
        StartCoroutine(SpawnEffect(effectTag, targetPoint, duration, isChild));
    }

    public IPlayerContext context;
    private bool isOfflineMode;

    private Spell spellD;
    private Spell spellF;

    public bool Controllable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    [Networked] private TickTimer delayD { get; set; }
    [Networked] private TickTimer delayF { get; set; }
}

