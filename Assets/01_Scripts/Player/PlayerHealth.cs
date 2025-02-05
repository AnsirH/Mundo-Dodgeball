using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
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
            Destroy(gameObject);
        }
    }

    public float maxHealth = 613.0f;
    public float health = 0.0f;

    public float healthRegenPerSec = 7.0f;
}
