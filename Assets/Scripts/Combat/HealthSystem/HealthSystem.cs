using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem
{
    private int maxHealth;
    private int health;

    public delegate void HealthChange();
    public static event HealthChange OnHealthChanged;

    public delegate void Defeat();

    public static event Defeat OnDefeat;

    public HealthSystem(int maxHealth, int currentHealth)
    {
        this.maxHealth = maxHealth;
        health = currentHealth;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealthPercent()
    {
        return (float) health / maxHealth;
    }

    public bool IsDefeated()
    {
        return health <= 0;
    }
    
    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0) health = 0;
        
        OnHealthChanged?.Invoke();
        
        if (health <= 0)
        {
            //health = 0;
            OnDefeat?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;


        OnHealthChanged?.Invoke();
    }
    
    public void SetHealth(int amount)
    {
        health = amount;
        if (health > maxHealth) health = maxHealth;
        if (health < 0) health = 0;
        
        OnHealthChanged?.Invoke();
        
        if (health <= 0)
        {
            //health = 0;
            OnDefeat?.Invoke();
        }
    }
}
