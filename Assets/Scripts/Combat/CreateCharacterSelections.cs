using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CreateCharacterSelections : ActionsMenu
{
    public GameObject buttonPrefab;

    public void Setup(List<CharacterBattle> characterBattles)
    {
        //Clear layout
        ClearLayout();
        //Clear button list
        ClearButtonsList();
        
        foreach (var character in characterBattles)
        {
            //Create button prefab, add to layout
            GameObject button = Instantiate(buttonPrefab, layout.transform);
            //Set text to character name
            button.GetComponent<EnemySelectButton>().SetText(character.unitStats.unitName);
            
            //Add listener for on click, to send reference to character battle back to battle handler
            button.GetComponent<Button>().onClick.AddListener((() =>
            {
                character.HideSelectionCircle();
                battleHandler.Attack(character);
                SetCurrentSelectionNull(); //Set current selection to null, such that no other buttons can be pressed
                CloseMenu(); //Close menu afterwards
            }));

            //Listener for OnSelect handled in prefab
            //On select is handled by reference to a character
            button.GetComponent<EnemySelectButton>().CharacterBattle = character;

            //Add to buttonObjects list
            buttonObjects.Add(button);
        }
    }
    
}
