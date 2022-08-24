using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnitAction : MonoBehaviour
{
    [SerializeField]
    private GameObject basicAttack;
    [SerializeField]
    private GameObject specialAttack; //Replace with list of available special attacks
    private GameObject currentAttack;
    
    [SerializeField]
    private Sprite faceSprite;
    void Awake () 
    {
        this.basicAttack = Instantiate(this.basicAttack, this.transform) as GameObject;
        this.specialAttack = Instantiate(this.specialAttack, this.transform) as GameObject;
        this.basicAttack.GetComponent<AttackTarget>().owner = this.gameObject;
        this.specialAttack.GetComponent<AttackTarget>().owner = this.gameObject;
        this.currentAttack = this.basicAttack; //current attack set to basic by default
    }
    
    //Replace with attack minigame object
    //If completed, call with attack target
    //Attack target subclass with a list of multipliers for performance on the minigame
    public void act (GameObject target) 
    {
        this.currentAttack.GetComponent<AttackTarget>().hit(target);
    }
    
    /*
     * Select current attack for this unit
     */
    public void selectAttack (bool isBasicAttack) 
    {
        this.currentAttack = (isBasicAttack) ? this.basicAttack : this.specialAttack;
    }
    
    public void updateHUD () 
    {
        GameObject playerUnitFace = GameObject.Find("PlayerUnitFace") as GameObject;
        playerUnitFace.GetComponent<Image>().sprite = this.faceSprite;
        GameObject playerUnitHealthBar = GameObject.Find("HealthBar") as GameObject;
        playerUnitHealthBar.GetComponent<ShowUnitHealth>().changeUnit(this.gameObject);
        GameObject playerUnitManaBar = GameObject.Find("CPBar") as GameObject;
        playerUnitManaBar.GetComponent<ShowUnitMana>().changeUnit(this.gameObject);
    }
}
