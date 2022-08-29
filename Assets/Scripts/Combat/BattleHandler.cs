using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class BattleHandler : MonoBehaviour
{
    private static BattleHandler instance;
    public static BattleHandler GetInstance()
    {
        return instance;
    }
    
    [SerializeField]
    private Transform combatUnit;

    public Sprite playerSpriteTest;
    public Sprite enemySpriteTest;

    [Header("Character Positions")] 
    //Likely to replace this with a file containing setups for different numbers of characters
    //(e.g. 1 character, 2 characters, 10 characters, etc)
    public List<Vector3> playerPositions;
    public List<Vector3> enemyPositions;

    [Header("Inputs")]
    private PlayerInput _playerInput;
    private bool isAttackPressed;

    [Header("Characters")] 
    public List<CharacterBattle> playerCharacterList;
    public List<CharacterBattle> playerCharacterCanActList;
    
    public List<CharacterBattle> enemyCharacterList;
    public List<CharacterBattle> enemyCharacterCanActList;

    private IPlayerParty _playerParty;
    private CharacterBattle activeCharacterBattle; //Character active

    [Header("UI")]
    private ActionsMenu _battleActionsMenu; //Menu for battle actions
    //Menu for selecting an attack
    //Menu for selecting a target
    
    [Header("Battle State")]
    private BattleState _state;
    private enum BattleState
    {
        //Waiting for player
        PlayerTurn,
        //Player performing an animation
        PlayerActing,
        EnemyTurn
    }
    
    private void Awake()
    {
        instance = this;
        
        //Handle receiving character sprites
        //Handle receiving character stats
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerParty = FindObjectOfType<IPlayerParty>();

        //Interpret PlayerParty object and list of unitstats
        //Construct list of player objects
        //Interpret EnemyEncounter object and list of unitstats
        //Construct list of enemy objects

        /*
         * Testing by adding 3 player characters and 3 enemy characters
         */
        for (int i = 0; i < 3; i++)
        {
            CharacterBattle player = SpawnCharacter(true, i);
            playerCharacterList.Add(player);
        }
        
        for (int i = 0; i < 3; i++)
        {
            CharacterBattle enemy = SpawnCharacter(false, i);
            enemyCharacterList.Add(enemy);
        }

        //Set all player act values to true (all players can act)
        SetAllPlayersCanAct();

        //Player turn first
        //Set active character to first character in player list
        SetActiveCharacter(playerCharacterList[0]);
        //Active character can be swapped by using Up and Down
        
        //Set state to PlayerTurn
        _state = BattleState.PlayerTurn;
        
        //Hide battle over window
        BattleOverWindow.instance.Hide();

        //Open battle actions menu
        _battleActionsMenu = FindObjectOfType<ActionsMenu>();
        _battleActionsMenu.OpenMenu();
    }

    private void Update()
    {
        
    }

    private CharacterBattle SpawnCharacter(bool isPlayerTeam, int positionIndex)
    {
        Vector3 position;
        if (isPlayerTeam)
        {
            position = playerPositions[positionIndex];
        }
        else
        {
            position = enemyPositions[positionIndex];
        }
        
        //Instantiate
        Transform characterTransform = Instantiate(combatUnit, position, Quaternion.identity);
        //Call setup function
        CharacterBattle characterBattle = characterTransform.GetComponent<CharacterBattle>();
        characterBattle.Setup(isPlayerTeam);

        return characterBattle;
    }

    public void PlayerAttack()
    {
        //If it's the player's turn and player is able to act
        if (_state == BattleState.PlayerTurn && playerCharacterCanActList.Contains(activeCharacterBattle))
        {
            _state = BattleState.PlayerActing;
            Debug.Log("Basic Attack");
            //Play attack animation
            
            //REPLACE LATER: 
            //Select random enemy in list and attack them
            ActiveCharacterAttackRandom(enemyCharacterList);
        }
    }

    private void SetActiveCharacter(CharacterBattle characterBattle)
    {
        if (activeCharacterBattle != null)
        {
            activeCharacterBattle.HideSelectionCircle();
        }
        activeCharacterBattle = characterBattle;
        activeCharacterBattle.ShowSelectionCircle();
    }

    private void ChooseNextActiveCharacter()
    {
        //Check if battle has ended
        if (TestBattleOver())
        {
            return;
        }
        
        //If active character was in the player act list
        if (playerCharacterCanActList.Contains(activeCharacterBattle))
        {
            PlayerActiveLast();
        }
        
        //If active character was in the enemy list
        else if(enemyCharacterList.Contains(activeCharacterBattle))
        {
            EnemyActiveLast();
        }
    }

    private void PlayerActiveLast()
    {
        //Remove active character from player act list
        playerCharacterCanActList.Remove(activeCharacterBattle);
        //Check if players can still act
        if (IsPlayerTurn())
        {
            //If true, set active character to the first player able to act
            SetActiveCharacter(playerCharacterCanActList[0]);
                
            _state = BattleState.PlayerTurn; //Set state back to player turn
            _battleActionsMenu.OpenMenu(); //Open battle actions menu again
        }
        //Else, pass turn to enemies
        else
        {
            //Set all enemies to be able to attack
            SetAllEnemiesCanAct();
                
            _state = BattleState.EnemyTurn; //Set state to enemy turn
            Debug.Log("Enemy Turn");
                
            //REPLACE LATER: enemy attack patterns play all at once
            //Each enemy in list attacks
            //Start with first enemy
            SetActiveCharacter(enemyCharacterCanActList[0]);
            ActiveCharacterAttackRandom(playerCharacterList);
        }
    }

    private void EnemyActiveLast()
    {
        //Remove active character from enemy act list
        enemyCharacterCanActList.Remove(activeCharacterBattle);
            
        //Check if enemies can still act
        if (IsEnemyTurn())
        {
            //If true, set active character to the next enemy able to act
            SetActiveCharacter(enemyCharacterCanActList[0]);
            ActiveCharacterAttackRandom(playerCharacterList);
        }
        //Else, pass turn to players
        else
        {
            //Set all players able to attack
            SetAllPlayersCanAct();
                
            SetActiveCharacter(playerCharacterCanActList[0]);
                
            _state = BattleState.PlayerTurn; //Set state back to player turn
            _battleActionsMenu.OpenMenu(); //Open battle actions menu again
        }
    }


    public void Attack(CharacterBattle targetCharacterBattle)
    {
        activeCharacterBattle.BasicAttack(targetCharacterBattle, delegate
        { 
            //After attack finished
            Debug.Log(activeCharacterBattle.name+" attack finished");
            ChooseNextActiveCharacter();
        });
    }
    

    /*
     * Basic attack to target a random character from a given list of characters
     */
    private void ActiveCharacterAttackRandom(List<CharacterBattle> characterList)
    {
        List<CharacterBattle> aliveCharacters = GetAliveCharacters(characterList);
        Debug.Log(aliveCharacters.Count);

        int randomInt = Random.Range(0, aliveCharacters.Count);
        Debug.Log(randomInt);
        CharacterBattle targetCharacterBattle = aliveCharacters[randomInt]; //Note: max value is exclusive
                
        activeCharacterBattle.BasicAttack(targetCharacterBattle, delegate
        { 
            //After attack finished
            Debug.Log(activeCharacterBattle.name+" attack finished");
            ChooseNextActiveCharacter();
        });
    }

    private bool TestBattleOver()
    {
        if (AllCharactersDefeated(playerCharacterList))
        {
            //Player dead, enemy wins
            
            BattleEnd(false);
            return true;
        }
        else if(AllCharactersDefeated(enemyCharacterList))
        {
            //Enemy dead, player wins
            
            BattleEnd(true);
            return true;
        }

        return false;
    }

    private void BattleEnd(bool playerWin)
    {
        if (playerWin)
        {
            Debug.Log("Player wins");
            BattleOverWindow.instance.Show(true);
            
            //Set unit stats for player party
            //Reward exp
            //In the battle over window, have buttons to go back to the main scene
        }
        else
        {
            Debug.Log("Enemy wins");
            BattleOverWindow.instance.Show(false);
        }
    }

    /*
     * Allow all players (given they are not defeated) to be able to act again
     */
    private void SetAllPlayersCanAct()
    {
        foreach (var pCharacterBattle in playerCharacterList)
        {
            //LATER
            //Maybe modify to not allow if character under some status effect
            if (!pCharacterBattle.IsDefeated())
            {
                playerCharacterCanActList.Add(pCharacterBattle);
            }
        }
        Debug.Log("All non-defeated players can act again");
        //playerCharacterCanActList = playerCharacterList;
    }

    private void SetAllEnemiesCanAct()
    {
        foreach (var eCharacterBattle in enemyCharacterList)
        {
            //LATER
            //Maybe modify to not allow if character under some status effect
            if (!eCharacterBattle.IsDefeated())
            {
                enemyCharacterCanActList.Add(eCharacterBattle);
            }
        }
        Debug.Log("All non-defeated enemies can act again");
        //enemyCharacterDict = enemyCharacterDict.ToDictionary(p => p.Key, p => true);
    }
    
    /*
     * Return if the player team can still act
     */
    private bool IsPlayerTurn()
    {
        return playerCharacterCanActList.Count != 0;
        //return playerCharacterDict.ContainsValue(true);
    }

    /*
     * Return if the enemy team can still act
     */
    private bool IsEnemyTurn()
    {
        return enemyCharacterCanActList.Count != 0;
        //return enemyCharacterDict.ContainsValue(true);
    }

    /*
     * Return if all the characters in a given list are defeated
     */
    private bool AllCharactersDefeated(List<CharacterBattle> characterBattles)
    {
        foreach (var character in characterBattles)
        {
            if (!character.IsDefeated()) return false;
        }
        return true;
    }

    /*
     * Return a list of characters from a list that aren't defeated
     */
    private List<CharacterBattle> GetAliveCharacters(List<CharacterBattle> characterBattles)
    {
        return characterBattles.Where(character => !character.IsDefeated()).ToList();
    }
}
