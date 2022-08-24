using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUnitMana : ShowUnitStat 
{
    override protected float newStatValue () 
    {
        return unit.GetComponent<UnitStats>().combatPoints;
    }

    protected override float newMaxStateValue()
    {
        return unit.GetComponent<UnitStats>().maxCombatPoints;
    }
}
