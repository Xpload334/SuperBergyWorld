using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateEnemyMenuItem : MonoBehaviour 
{
    [SerializeField]
    private GameObject targetEnemyUnitPrefab;
    [SerializeField]
    private Sprite menuItemSprite;
    [SerializeField]
    private KillEnemy killEnemyScript;
    // Use this for initialization
    void Awake () 
    {
        GameObject enemyUnitsMenu = GameObject.Find("EnemyUnitsMenu");
        GameObject targetEnemyUnit = Instantiate(this.targetEnemyUnitPrefab, enemyUnitsMenu.transform) as GameObject;
        targetEnemyUnit.name = "Target" + this.gameObject.name;
        targetEnemyUnit.GetComponent<Button>().onClick.AddListener(() => selectEnemyTarget());
        targetEnemyUnit.GetComponent<Image>().sprite = this.menuItemSprite;
        killEnemyScript.menuItem = targetEnemyUnit;
    }
    /*
     * attackEnemyTarget() from TurnSystem is called here
     * Finds party manager object
     */
    public void selectEnemyTarget () 
    {
        GameObject partyData = GameObject.Find("PlayerParty");
        partyData.GetComponent<SelectUnit>().attackEnemyTarget(this.gameObject);
    }
}
