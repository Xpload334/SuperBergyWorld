using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class UnitStats : MonoBehaviour, IComparable
{
    public bool isPlayerTeam;
    [Header("Attacks (Player)")] 
    public GameObject basicAttack;
    public List<GameObject> specialAttacks; //Replace with attack prefab

    [Header("Attacks (If Enemy)")] 
    public List<GameObject> attackPatterns;
    
    [Header("Effects")]
    public Animator animator;
    public string unitName;
    
    [Header("Stats")]
    //Health system here
    public int maxHealth; //maximum health
    public int health;
    
    public int level; //level determines attack damage
    public int experience;
    
    //Combat points system here
    public int combatPoints;
    public int maxCombatPoints; //maximum amount of combat points
    
    public int defence;
    public int speed;

    public int nextActTurn;
    //private bool dead = false;
    

    public void calculateNextActTurn (int currentTurn)
    {
        /*
        this.nextActTurn = currentTurn + (int)Math.Ceiling(100.0f / this.speed);
        */
    }
    public int CompareTo (object otherStats)
    {
        return nextActTurn.CompareTo(((UnitStats)otherStats).nextActTurn);
    }
    
    public bool isDead ()
    {
        return false;
    }
    
    /*
     * Make this unit take damage
     *
     *
     * (May remove everything except reducing health, as damage text and animations are handled by the CharacterBattle script)
     */
    public void ReceiveDamage (int damage) 
    {
        this.health -= damage;
        //animator.Play("Hit");
        //GameObject HUDCanvas = GameObject.Find("HUDCanvas");
        //GameObject damageText = Instantiate(this.damageTextPrefab, HUDCanvas.transform) as GameObject;
        //damageText.GetComponent<Text>().text = "" + damage;
        //damageText.transform.localPosition = this.damageTextPosition;
        //damageText.transform.localScale = new Vector2(1.0f, 1.0f);
        
        //DamageText.Create(damageTextPosition, damage, false);
        
        /*
        if(this.health <= 0)
        {
            this.dead = true;
            this.gameObject.tag = "DeadUnit";
            Destroy(this.gameObject);
        }
        */
    }
    
    public void ReceiveExperience (int gainedExperience) 
    {
        this.experience += gainedExperience;
    }
}
