using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPointsSystem
{
    private int maxCombatPoints;
    private int combatPoints;

    public delegate void CPChange();
    public static event CPChange OnCPChanged;

    public CombatPointsSystem(int maxCombatPoints)
    {
        this.maxCombatPoints = maxCombatPoints;
        combatPoints = maxCombatPoints;
    }

    public int GetCP()
    {
        return combatPoints;
    }

    public int GetMaxCP()
    {
        return maxCombatPoints;
    }

    public float GetCPPercent()
    {
        return (float) combatPoints / maxCombatPoints;
    }
    
    public void SpendCP(int amount)
    {
        combatPoints -= amount;
        if (combatPoints < 0) combatPoints = 0;

        OnCPChanged?.Invoke();
    }

    public void GainCP(int amount)
    {
        combatPoints += amount;
        if (combatPoints > maxCombatPoints) combatPoints = maxCombatPoints;

        OnCPChanged?.Invoke();
    }
}
