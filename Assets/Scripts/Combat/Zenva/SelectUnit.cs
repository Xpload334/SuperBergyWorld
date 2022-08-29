using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectUnit : MonoBehaviour
{
    private GameObject currentUnit;
    private ActionsMenu actionsMenu;
    private IEnemyUnitsMenu enemyUnitsMenu;
    void Awake () 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded (Scene scene, LoadSceneMode mode) 
    {
        if(scene.name == Scenes.BattleScene) 
        {
            //Find menus once battle scene loads
            this.actionsMenu = FindObjectOfType<ActionsMenu>();
            this.enemyUnitsMenu = FindObjectOfType<IEnemyUnitsMenu>();
        }
    }
    
    /*
     * Select a unit as the current unit
     * Enable actions menu
     */
    public void selectCurrentUnit (GameObject unit) 
    {
        this.currentUnit = unit;
        this.actionsMenu.gameObject.SetActive(true);
        this.currentUnit.GetComponent<PlayerUnitAction>().updateHUD();
    }
    
    /*
     * Call select attack for the current unit
     * Change the menu to the enemyUnitsMenu
     *
     * Basically, once player selected attack, can now select target
     */
    public void selectAttack(bool isBasicAttack) 
    {
        this.currentUnit.GetComponent<PlayerUnitAction>().selectAttack(isBasicAttack);
        this.actionsMenu.gameObject.SetActive(false);
        this.enemyUnitsMenu.gameObject.SetActive(true);
    }
    
    /*
     * Disable both menus and call act method for current unit, with selected enemy as target
     */
    public void attackEnemyTarget (GameObject target) 
    {
        this.actionsMenu.gameObject.SetActive(false);
        this.enemyUnitsMenu.gameObject.SetActive(false);
        this.currentUnit.GetComponent<PlayerUnitAction>().act(target);
    }
}
