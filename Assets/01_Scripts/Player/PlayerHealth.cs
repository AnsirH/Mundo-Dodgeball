using Photon.Pun;
using PlayerCharacterControl.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviourPunCallbacks, IDamageable, IPunObservable
{
    private void Awake()
    {
        health = maxHealth;
    }

    private void Update()
    {
        HealthRegen();   
    }

    private void HealthRegen()
    {
        health += healthRegenPerSec * Time.deltaTime;

        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public void Damage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    private void DieRPC()
    {
        gameObject.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(health);
        else
            health = (float)stream.ReceiveNext();
    }

    public float maxHealth = 613.0f;
    public float health = 0.0f;

    public float healthRegenPerSec = 7.0f;
}
