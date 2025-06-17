using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class EffectObject : NetworkBehaviour
{
    [Networked] TickTimer LifeTimer {  get; set; }

    public ParticleSystem particle;

    public bool isInitialize = false;

    public override void Spawned()
    {
        Init();
    }

    public virtual void Init()
    {
        if (particle != null)
        {
            LifeTimer = TickTimer.CreateFromSeconds(Runner, Mathf.Max(particle.main.duration, particle.main.duration + particle.main.startLifetime.constantMax));
            isInitialize = true;
        }
        else
        {
            CallDespawn_RPC();
        }
    }

    public override void Render()
    {
        if (isInitialize && LifeTimer.ExpiredOrNotRunning(Runner))
            CallDespawn_RPC();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void CallDespawn_RPC()
    {
        Runner.Despawn(Object);
    }
}
