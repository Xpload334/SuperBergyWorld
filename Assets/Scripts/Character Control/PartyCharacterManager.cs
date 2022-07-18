using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Manages the list of characters in the party
//Characters should really be children of the object this script is attached to
//Uses input system to allow to swap characters
public class PartyCharacterManager : MonoBehaviour
{
    public PlayerController currentCharacter;
    private int currentCharacterIndex;
    public List<PlayerController> characters; //Also add as children
    //Change: maybe read in the child objects of this

    private int partySize;
    
    //Instance of PlayerInput
    [Header("Player Input")] 
    public bool ShouldSwap;
    public PlayerInput input;

    
    // Start is called before the first frame update
    void Start()
    {
        initialiseParty();
    }

    void initialiseParty()
    {
        //If characters is empty
        if (characters.Count == 0)
        {
            //If current character is set
            if (currentCharacter != null)
            {
                characters.Add(currentCharacter);
            }
            else
            {
                Debug.LogError("Party is empty.");
            }
        }
        
        partySize = characters.Count;
        
        //If current character set, and contained in characters
        if (currentCharacter != null && characters.Contains(currentCharacter))
        {
            //Get index of current character
            currentCharacterIndex = characters.IndexOf(currentCharacter);
        }
        else
        {
            //Set to first character in the party
            currentCharacterIndex = 0;
            currentCharacter = characters[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Cycle between characters in the party
    //Usually bound to E
    //Do I potentially want Q and E to do Previous and Next? (Consult team)
    void swapCharacters()
    {
        if (currentCharacterIndex < (partySize - 1))
        {
            //Set current character state to follow
            //Increment index
            //Get character at new index
            //Set new character to current character
            //Set new character state to active (allow control)
        }
    }
    
}
