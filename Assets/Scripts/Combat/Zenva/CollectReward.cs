using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Player receives reward from enemy encounters
 */
public class CollectReward : MonoBehaviour
{
    [SerializeField]
    private float experience; //experience reward
    public void Start () 
    {
        Debug.Log("Enemy encounter set");
    }
    
    /*
     * Divide experience reward between all living player units
     */
    public void collectReward () 
    {
        GameObject[] livingPlayerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");
        int experiencePerUnit = (int)(this.experience / (float)livingPlayerUnits.Length);
        
        foreach(GameObject playerUnit in livingPlayerUnits) 
        {
            playerUnit.GetComponent<UnitStats>().ReceiveExperience(experiencePerUnit);
        }
        Destroy(this.gameObject);
    }
}
