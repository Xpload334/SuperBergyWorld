using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UnitStats : MonoBehaviour, IComparable
{
    public Animator animator;
    //Damage text prefab
    public GameObject damageTextPrefab;
    public Vector3 damageTextPosition;
    
    
    public string unitName;
    public float maxHealth; //maximum health
    public float health;
    public float level; //level determines attack damage
    public float experience;
    public float combatPoints;
    public float maxCombatPoints; //maximum amount of combat points
    public float defence;
    public float speed;

    public int nextActTurn;
    private bool dead = false;
    

    public void calculateNextActTurn (int currentTurn)
    {
        this.nextActTurn = currentTurn + (int)Math.Ceiling(100.0f / this.speed);
    }
    public int CompareTo (object otherStats)
    {
        return nextActTurn.CompareTo(((UnitStats)otherStats).nextActTurn);
    }
    public bool isDead ()
    {
        return this.dead;
    }
    
    /*
     * Make this unit take damage
     *
     */
    public void receiveDamage (float damage) 
    {
        this.health -= damage;
        animator.Play("Hit");
        GameObject HUDCanvas = GameObject.Find("HUDCanvas");
        GameObject damageText = Instantiate(this.damageTextPrefab, HUDCanvas.transform) as GameObject;
        damageText.GetComponent<Text>().text = "" + damage;
        damageText.transform.localPosition = this.damageTextPosition;
        damageText.transform.localScale = new Vector2(1.0f, 1.0f);
        
        if(this.health <= 0)
        {
            this.dead = true;
            this.gameObject.tag = "DeadUnit";
            Destroy(this.gameObject);
        }
    }
    
    public void ReceiveExperience (float gainedExperience) 
    {
        this.experience += gainedExperience;
    }
}
