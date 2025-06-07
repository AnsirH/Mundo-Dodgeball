using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpell : MonoBehaviour, IPlayerComponent
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
    private void SpawnEffect_RPC(string effectTag, Vector3 targetPoint, float duration = 1.0f, bool isChild = false)
    {
        StartCoroutine(SpawnEffect(effectTag, targetPoint, duration, isChild));
    }

    public IPlayerContext context;
    private bool isOfflineMode;

    private Spell spellD;
    private Spell spellF;

    public bool Controllable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}

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

public class Flash : Spell
{
    public Flash(IPlayerContext context) : base(context)
    {
        this.context = context;
    }

    public override void Execute()
    {
        Vector3 targetPoint = context.MousePositionGetter.ClickPoint.Value;
        Vector3 direction = (context.MousePositionGetter.ClickPoint.Value - context.Trf.position).normalized;

        context.Trf.position = targetPoint;
        context.Trf.rotation = Quaternion.LookRotation(direction);


        currentCoolTime = maxCoolTime;
    }

    float distance = 3.0f;
}

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