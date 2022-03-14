using UnityEngine;

using System;

public class LivingEntity : MonoBehaviour, IDamageable
{

    public float initialHealth = 100f;
    public float health { get; protected set; }

    protected bool dead;

    public event Action OnDeath;

    protected virtual void Start()
    {
        health = initialHealth;
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDir)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        dead = true;

        if (OnDeath != null)
        {
            OnDeath();
        }

        Destroy(gameObject);
    }

} // class
