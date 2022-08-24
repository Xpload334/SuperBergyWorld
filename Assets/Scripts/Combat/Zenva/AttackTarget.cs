using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackTarget : MonoBehaviour
{
    public GameObject owner;
    [SerializeField]
    private string attackAnimation;
    [SerializeField]
    private bool specialAttack;
    [SerializeField]
    private float manaCost;
    
    [SerializeField]
    private float attackBaseDamage;
    [SerializeField]
    private float attackLevelMultiplier = 0.5f;
    [SerializeField] 
    private float defenceMultiplier = 1;
  
    public void hit(GameObject target) 
    {
        UnitStats ownerStats = this.owner.GetComponent<UnitStats>();
        UnitStats targetStats = target.GetComponent<UnitStats>();
        if(ownerStats.combatPoints >= this.manaCost) 
        {
            //Removed random damage variance
            //Damage calculation
            float damage = calculateDamage(ownerStats, targetStats);


            this.owner.GetComponent<Animator>().Play(this.attackAnimation);
            targetStats.receiveDamage(damage);
            ownerStats.combatPoints -= this.manaCost;
        }
    }

    private float calculateDamage(UnitStats ownerStats, UnitStats targetStats)
    {
        float damage = attackBaseDamage * (attackLevelMultiplier * ownerStats.level);
        
        damage = Mathf.Max(0, damage - (this.defenceMultiplier * targetStats.defence));

        return damage;
    }
}
