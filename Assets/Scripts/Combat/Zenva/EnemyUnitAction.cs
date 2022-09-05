using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitAction : MonoBehaviour
{
    [SerializeField]
    private GameObject attack;
    [SerializeField]
    private string targetsTag;
    void Awake () 
    {
        this.attack = Instantiate(this.attack);
        this.attack.GetComponent<AttackTarget>().owner = this.gameObject;
    }
    GameObject findRandomTarget () 
    {
        GameObject[] possibleTargets = GameObject.FindGameObjectsWithTag(targetsTag);
        if(possibleTargets.Length > 0) 
        {
            int targetIndex = Random.Range(0, possibleTargets.Length);
            GameObject target = possibleTargets [targetIndex];
            return target;
        }
        return null;
    }
    
    //Replace hit with attack sequence
    //If player is hit, call the hit method for damage
    public void act () 
    {
        GameObject target = findRandomTarget();
        //this.attack.GetComponent<AttackTarget>().Hit(target);
    }
}
