using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnSystem : MonoBehaviour
{
    private List<UnitStats> unitsStats;
    [SerializeField]
    private GameObject actionsMenu, enemyUnitsMenu;

    [SerializeField] 
    public GameObject enemyEncounter;

    public GameObject playerParty;
    void Start ()
    {
        unitsStats = new List<UnitStats>();
        GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");
        
        //Player units attack based on their actTurn number, which is like speed
        foreach(GameObject playerUnit in playerUnits) 
        {
            UnitStats currentUnitStats = playerUnit.GetComponent<UnitStats>();
            currentUnitStats.calculateNextActTurn(0);
            unitsStats.Add(currentUnitStats);
        }
        GameObject[] enemyUnits = GameObject.FindGameObjectsWithTag("EnemyUnit");
        foreach(GameObject enemyUnit in enemyUnits) 
        {
            UnitStats currentUnitStats = enemyUnit.GetComponent<UnitStats>();
            currentUnitStats.calculateNextActTurn(0);
            unitsStats.Add(currentUnitStats);
        }
        unitsStats.Sort();
        this.actionsMenu.SetActive(false);
        this.enemyUnitsMenu.SetActive(false);
        this.nextTurn();
    }
    
    public void nextTurn () 
    {
        GameObject[] remainingEnemyUnits = GameObject.FindGameObjectsWithTag("EnemyUnit");
    
        //If no enemies remaining, battle won
        if(remainingEnemyUnits.Length == 0) 
        {
            Debug.Log("Battle won");
            this.enemyEncounter.GetComponent<CollectReward>().collectReward();
            SceneManager.LoadScene(Scenes.Overworld);
        }
        GameObject[] remainingPlayerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");
    
        //If no players remaining, battle lost
        if(remainingPlayerUnits.Length == 0) 
        {
            Debug.Log("Battle lost");
            SceneManager.LoadScene(Scenes.TitleScene);
        }

        //Get unit with earliest act turn stat
        UnitStats currentUnitStats = unitsStats[0];
        unitsStats.Remove(currentUnitStats);
        
        //If current unit isn't dead
        if(!currentUnitStats.isDead()) 
        {
            GameObject currentUnit = currentUnitStats.gameObject;
            currentUnitStats.calculateNextActTurn(currentUnitStats.nextActTurn);
            unitsStats.Add(currentUnitStats);
            unitsStats.Sort();
            if(currentUnit.CompareTag("PlayerUnit"))
            {
                //If player, do player acting methods
                Debug.Log("Player unit acting");
                this.playerParty.GetComponent<SelectUnit>().selectCurrentUnit(currentUnit.gameObject);
            } 
            else 
            {
                //If enemy, do enemy attack methods
                Debug.Log("Enemy unit acting");
                currentUnit.GetComponent<EnemyUnitAction>().act();
            }
        } 
        else 
        {
            //Recursion
            this.nextTurn();
        }
    }
}
