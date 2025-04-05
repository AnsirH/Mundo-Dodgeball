using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviourPunCallbacks, IPunObservable, IDamageable, IPlayerComponent
{
    private IPlayerContext context;
    private float currentHealth;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthRegenPerSec = 7.0f;

    public void Initialize(IPlayerContext context)
    {
        this.context = context;
        currentHealth = maxHealth;
    }

    private void HealthRegen()
    {
        currentHealth += healthRegenPerSec * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void Damage(float damage)
    {
        if (!context.IsLocalPlayer()) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        if (currentHealth <= 0)
        {
            context.OnPlayerDeath();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(currentHealth);
        else
            currentHealth = (float)stream.ReceiveNext();
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public void Updated()
    {
        HealthRegen();
    }

    public void OnEnabled()
    {
        //throw new System.NotImplementedException();
    }

    public void OnDisabled()
    {
        //throw new System.NotImplementedException();
    }
}
