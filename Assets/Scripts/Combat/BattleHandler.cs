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
    [Header("Test Stats")] 
    public UnitStats playerStats;
    public UnitStats enemyStats;
    
    
    private static BattleHandler _instance;
    public static BattleHandler GetInstance()
    {
        return _instance;
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
    private bool _isAttackPressed;

    [Header("Player Team")] 
    public List<CharacterBattle> playerCharacterList;
    public List<CharacterBattle> playerCharacterCanActList;
    private IPlayerParty _playerParty;
    private Transform _playerPartyTransform;
    
    [Header("Enemy Team")]
    public List<CharacterBattle> enemyCharacterList;
    public List<CharacterBattle> enemyCharacterCanActList;
    private Transform _enemyEncounterTransform;

    
    private CharacterBattle _activeCharacterBattle; //Character active

    [Header("UI")]
    [SerializeField] private ActionsMenu battleActionsMenu; //Menu for battle actions
    [SerializeField] private ActionsMenu attackSelectionMenu; //Menu for selecting an attack
    [SerializeField] private CreateCharacterSelections targetSelectionMenu; //Menu for selecting a target
    
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
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerPartyTransform = FindObjectOfType<IPlayerParty>().transform;
        _enemyEncounterTransform = FindObjectOfType<EnemyEncounter>().transform;

        //Interpret PlayerParty object and list of unitstats
        int p = 0;
        GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");
        foreach(GameObject playerUnit in playerUnits) 
        {
            UnitStats currentUnitStats = playerUnit.GetComponent<UnitStats>();
            CharacterBattle player = SpawnCharacter(true, p, currentUnitStats);
            playerCharacterList.Add(player);
            p++;
        }
        //Interpret EnemyEncounter object and list of unitstats
        int e = 0;
        GameObject[] enemyUnits = GameObject.FindGameObjectsWithTag("EnemyUnit");
        foreach(GameObject enemyUnit in enemyUnits) 
        {
            UnitStats currentUnitStats = enemyUnit.GetComponent<UnitStats>();
            CharacterBattle enemy = SpawnCharacter(false, e, currentUnitStats);
            enemyCharacterList.Add(enemy);
            e++;
        }
        /*
        foreach (Transform character in _playerPartyTransform)
        {
            UnitStats unitStats = character.GetComponent<UnitStats>();
            //Construct player object
            CharacterBattle player = SpawnCharacter(true, p, unitStats);
            playerCharacterList.Add(player);
            p++;
        }
        */
        
        
        /*
        foreach (Transform character in _enemyEncounterTransform)
        {
            UnitStats unitStats = character.GetComponent<UnitStats>();
            //Construct enemy object
            CharacterBattle enemy = SpawnCharacter(false, e, unitStats);
            enemyCharacterList.Add(enemy);
            e++;
        }
        */
        
        /*
         * Testing by adding 3 player characters and 3 enemy characters
         */
        /*
        for (int i = 0; i < 3; i++)
        {
            CharacterBattle player = SpawnCharacter(true, i, playerStats);
            playerCharacterList.Add(player);
        }
        for (int i = 0; i < 3; i++)
        {
            CharacterBattle enemy = SpawnCharacter(false, i, enemyStats);
            enemyCharacterList.Add(enemy);
        }
        */

        //Set all player act values to true (all players can act)
        SetAllPlayersCanAct();

        //Player turn first
        //Set active character to first character in player list
        SetActiveCharacter(playerCharacterList[0]);
        //Active character can be swapped by using Up and Down
        
        //Set state to PlayerTurn
        _state = BattleState.PlayerTurn;
        
        //Hide battle over window
        BattleOverWindow.Instance.Hide();

        //Open battle actions menu
        battleActionsMenu.battleHandler = this;
        battleActionsMenu.OpenMenu();
        
        //Hide attack selection menu
        
        //Hide target selection menu
        targetSelectionMenu.battleHandler = this;
        targetSelectionMenu.CloseMenu();
    }

    private CharacterBattle SpawnCharacter(bool isPlayerTeam, int positionIndex, UnitStats unitStats)
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
        characterBattle.Setup(isPlayerTeam, unitStats);

        return characterBattle;
    }

    /*
     * On player attack selected:
     *
     * Go to attack selection menu
     */
    public void PlayerAttack()
    {
        //If it's the player's turn and player is able to act
        if (_state == BattleState.PlayerTurn && playerCharacterCanActList.Contains(_activeCharacterBattle))
        {
            Debug.Log("Player Attack Selected");
            targetSelectionMenu.Setup(GetAliveCharacters(enemyCharacterList));
            
            targetSelectionMenu.OpenMenu();
        }
    }

    public void AttackTargetCharacterBattle(CharacterBattle targetCharacterBattle)
    {
        _activeCharacterBattle.BasicAttack(targetCharacterBattle, delegate
        { 
            //After attack finished
            Debug.Log(_activeCharacterBattle.name+" attack finished");
            ChooseNextActiveCharacter();
        });
    }

    private void SetActiveCharacter(CharacterBattle characterBattle)
    {
        if (_activeCharacterBattle != null)
        {
            _activeCharacterBattle.HideSelectionCircle();
        }
        _activeCharacterBattle = characterBattle;
        _activeCharacterBattle.ShowSelectionCircle();
    }

    private void ChooseNextActiveCharacter()
    {
        //Check if battle has ended
        if (TestBattleOver())
        {
            return;
        }
        
        //If active character was in the player act list
        if (playerCharacterCanActList.Contains(_activeCharacterBattle))
        {
            PlayerActiveLast();
        }
        
        //If active character was in the enemy list
        else if(enemyCharacterList.Contains(_activeCharacterBattle))
        {
            EnemyActiveLast();
        }
    }

    private void PlayerActiveLast()
    {
        //Remove active character from player act list
        playerCharacterCanActList.Remove(_activeCharacterBattle);
        //Check if players can still act
        if (IsPlayerTurn())
        {
            //If true, set active character to the first player able to act
            SetActiveCharacter(playerCharacterCanActList[0]);
                
            _state = BattleState.PlayerTurn; //Set state back to player turn
            battleActionsMenu.OpenMenu(); //Open battle actions menu again
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
        enemyCharacterCanActList.Remove(_activeCharacterBattle);
            
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
            battleActionsMenu.OpenMenu(); //Open battle actions menu again
        }
    }
    
    public void Attack(CharacterBattle targetCharacterBattle)
    {
        _activeCharacterBattle.BasicAttack(targetCharacterBattle, delegate
        { 
            //After attack finished
            Debug.Log(_activeCharacterBattle.name+" attack finished");
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
                
        AttackTargetCharacterBattle(targetCharacterBattle);
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
            BattleOverWindow.Instance.Show(true);
            
            //Set unit stats for player party
            //Reward exp
            //In the battle over window, have buttons to go back to the main scene
        }
        else
        {
            Debug.Log("Enemy wins");
            BattleOverWindow.Instance.Show(false);
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

    public void ShowSelectionCircle(CharacterBattle characterBattle)
    {
        characterBattle.ShowSelectionCircle();
    }

    public void HideSelectionCircle(CharacterBattle characterBattle)
    {
        characterBattle.HideSelectionCircle();
    }
}
