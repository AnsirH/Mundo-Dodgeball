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
            }
        }
        if (context.action.name == "F")
        {
            if (spellF.CanUsable)
            {
                spellF.Execute();
                StartCoroutine(SpawnEffect("FlashEffect", this.context.MousePositionGetter.ClickPoint.Value));
            }
        }
    }


    private IEnumerator SpawnEffect(string effectTag, Vector3 targetPoint)
    {
        GameObject effect = ObjectPooler.Get(effectTag);

        if (effect == null)
        {
            Debug.LogError($"Effect with tag {effectTag} not found in ObjectPooler.");
            yield break;
        }

        targetPoint.y = IngameController.Instance.ground.transform.position.y;
        effect.transform.position = targetPoint;

        yield return new WaitForSeconds(1.0f);

        ObjectPooler.Release(effectTag, effect);
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
        Vector3 direction = (context.MousePositionGetter.ClickPoint.Value - context.Pos).normalized;

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