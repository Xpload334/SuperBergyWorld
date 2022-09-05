using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem
{
    private int _maxHealth;
    private int _health;

    public delegate void HealthChange();
    public static event HealthChange OnHealthChanged;

    public delegate void Defeat();

    public static event Defeat OnDefeat;

    public HealthSystem(int maxHealth, int currentHealth)
    {
        this._maxHealth = maxHealth;
        _health = currentHealth;
    }

    public int GetHealth()
    {
        return _health;
    }

    public int GetMaxHealth()
    {
        return _maxHealth;
    }

    public float GetHealthPercent()
    {
        return (float) _health / _maxHealth;
    }

    public bool IsDefeated()
    {
        return _health <= 0;
    }
    
    public void Damage(int amount)
    {
        _health -= amount;
        if (_health < 0) _health = 0;
        
        OnHealthChanged?.Invoke();
        
        if (_health <= 0)
        {
            //health = 0;
            OnDefeat?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        _health += amount;
        if (_health > _maxHealth) _health = _maxHealth;


        OnHealthChanged?.Invoke();
    }
    
    public void SetHealth(int amount)
    {
        _health = amount;
        if (_health > _maxHealth) _health = _maxHealth;
        if (_health < 0) _health = 0;
        
        OnHealthChanged?.Invoke();
        
        if (_health <= 0)
        {
            //health = 0;
            OnDefeat?.Invoke();
        }
    }
}
