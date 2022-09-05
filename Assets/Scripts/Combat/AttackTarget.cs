using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackTarget : MonoBehaviour
{
    public GameObject owner;
    //private string attackAnimation;
    [SerializeField]
    private float manaCost;
    
    [SerializeField]
    private float attackBaseDamage; //Minimum attack damage
    [SerializeField]
    private float attackLevelMultiplier = 0.5f; //Extra damage multiplier based on level
    [SerializeField] 
    private float defenceMultiplier = 1;
    

    public int CalculateDamage(UnitStats ownerStats, UnitStats targetStats)
    {
        float damage = attackBaseDamage + (attackLevelMultiplier * (ownerStats.level - 1));
        
        int damageInt = Mathf.RoundToInt(Mathf.Max(0, damage - (this.defenceMultiplier * targetStats.defence)));

        return damageInt;
    }
}
