using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPointsSystem
{
    private int _maxCombatPoints;
    private int _combatPoints;

    public delegate void CpChange();
    public static event CpChange OnCpChanged;

    public CombatPointsSystem(int maxCombatPoints)
    {
        this._maxCombatPoints = maxCombatPoints;
        _combatPoints = maxCombatPoints;
    }

    public int GetCp()
    {
        return _combatPoints;
    }

    public int GetMaxCp()
    {
        return _maxCombatPoints;
    }

    public float GetCpPercent()
    {
        return (float) _combatPoints / _maxCombatPoints;
    }
    
    public void SpendCp(int amount)
    {
        _combatPoints -= amount;
        if (_combatPoints < 0) _combatPoints = 0;

        OnCpChanged?.Invoke();
    }

    public void GainCp(int amount)
    {
        _combatPoints += amount;
        if (_combatPoints > _maxCombatPoints) _combatPoints = _maxCombatPoints;

        OnCpChanged?.Invoke();
    }
}
