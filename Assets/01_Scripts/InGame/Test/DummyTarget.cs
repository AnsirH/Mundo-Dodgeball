using Fusion;
using UnityEngine;

public class DummyTarget : NetworkBehaviour, IDamageable
{
    [Networked] private float _hp { get;set; }

    public override void Spawned()
    {
        _hp = 50.0f;
    }

    public void TakeDamage(float damage)
    {
        _hp += -damage;
    }

    public override void FixedUpdateNetwork()
    {
        if (_hp < 0) Runner.Despawn(Object);
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(0, 300, 100, 300), "Dummy Target HP:" + _hp);
    }
}
