using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCharacterSelections : ActionsMenu
{
    public GameObject buttonPrefab;
    /*
     * For each character battle
     *
     * Create button
     * Set text to character name
     *
     * Add listener for on select (will use later)
     * Add listener for on click, to send reference to character battle back to battle handler
     *
     * Add to buttonObjects list
     */


    public void Setup(List<CharacterBattle> characterBattles)
    {
        //Clear layout
        ClearLayout();
        foreach (var character in characterBattles)
        {
            //Create button prefab
            
            //Set text to character name
            
            //Add listener for on select (will use later)
            
            //Add listener for on click, to send reference to character battle back to battle handler
            
            //Add to buttonObjects list
            
            //Add to layout
        }
    }

    private void ClearLayout()
    {
        foreach (Transform child in layout.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    
    
    
}
