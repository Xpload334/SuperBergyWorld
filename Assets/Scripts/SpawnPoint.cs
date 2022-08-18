using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool TriggerOnSceneLoad;
    public bool MoveAllCharacters;

    public float Priority = 10f;

    private PartyCharacterManager _partyCharacterManager;

    // Start is called before the first frame update
    void Start()
    {
        _partyCharacterManager = FindObjectOfType<PartyCharacterManager>();
        
        if (TriggerOnSceneLoad)
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveCharacters()
    {
        
    }

    void MoveCharacter()
    {
        
    }
}
