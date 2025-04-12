using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyGame.Utils;
using UnityEngine.Diagnostics;

public class PlayerSpell : MonoBehaviour, IPlayerComponent
{
    private void Awake()
    {
        spellD = new Flash(this);
    }

    public void OnSpellD()
    {
        if (canUseSpellD)
        {
            spellD.Execute();
        }
    }

    private void SetSpellable()
    {
        canUseSpellD = true;
    }

    public void Initialize(IPlayerContext context)
    {
        throw new System.NotImplementedException();
    }

    public void Updated()
    {
        if (!spellD.CanUsable)
        {
            spellD.CoolDown(Time.deltaTime);
        }
    }

    public void OnEnabled()
    {
        throw new System.NotImplementedException();
    }

    public void OnDisabled()
    {
        throw new System.NotImplementedException();
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    private Spell spellD;

    public float flashDistance = 1.5f;
    public bool canUseSpellD = true;

    public bool Controllable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}

public class Spell
{
    public PlayerSpell owner;
    public Spell(PlayerSpell owner)
    {
        this.owner = owner;
    }

    public virtual void Execute()
    {
        if (!CanUsable) { return; }
    }

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
    public Flash(PlayerSpell owner) : base(owner)
    {
    }

    public override void Execute()
    {
        base.Execute();
        Vector3 targetVector = Utility.GetMousePosition(Camera.main).Value - owner.transform.position;
        if (targetVector.magnitude > distance)
        {
            owner.transform.position += targetVector.normalized * distance;
        }
        else
        {
            owner.transform.position += targetVector;
        }
        owner.transform.rotation = Quaternion.LookRotation(targetVector);

        currentCoolTime = maxCoolTime;
    }

    float distance = 3.0f;
}
