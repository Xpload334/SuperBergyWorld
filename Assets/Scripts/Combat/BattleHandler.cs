using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private PlayerInput _playerInput;
    private bool isAttackPressed;
    
    private List<CharacterBattle> characterBattleList;
    private CharacterBattle playerCharacterBattle;
    private CharacterBattle enemyCharacterBattle;
    private CharacterBattle activeCharacterBattle; //Character active

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
        
        //Handle recieving character sprites
        //Handle recieving characrer stats
        
        
        //Player input
        _playerInput = new PlayerInput();
        
        _playerInput.CombatControls.BasicAttack.started += OnBasicAttack;
        _playerInput.CombatControls.BasicAttack.canceled += OnBasicAttack;

        //_playerInput.CombatControls.BasicAttack.started += OnBasicAttack;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        playerCharacterBattle = SpawnCharacter(true);
        enemyCharacterBattle = SpawnCharacter(false);
        
        //Player turn first
        SetActiveCharacter(playerCharacterBattle);
        _state = BattleState.PlayerTurn;
        
        BattleOverWindow.instance.Hide();
    }

    private void Update()
    {
        
    }

    private CharacterBattle SpawnCharacter(bool isPlayerTeam)
    {
        Vector3 position;
        if (isPlayerTeam)
        {
            position = new Vector3(-5, 1, 0);
        }
        else
        {
            position = new Vector3(5, 1, 0);
        }
        
        //Instantiate
        Transform characterTransform = Instantiate(combatUnit, position, Quaternion.identity);
        //Call setup function
        CharacterBattle characterBattle = characterTransform.GetComponent<CharacterBattle>();
        characterBattle.Setup(isPlayerTeam);

        return characterBattle;
    }

    void OnBasicAttack(InputAction.CallbackContext ctx)
    {
        /*
         * Definitely change this later such that attacks switch to enemy turn once all players acted
         */
        isAttackPressed = ctx.ReadValueAsButton();
        if(!isAttackPressed) return;

        if (_state == BattleState.PlayerTurn)
        {
            _state = BattleState.PlayerActing;
            Debug.Log("Basic Attack");
            //Play attack animation
            playerCharacterBattle.BasicAttack(enemyCharacterBattle, delegate
            {
                //After attack finished
                Debug.Log("Attack finished");
                ChooseNextActiveCharacter();
            });
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
        if (TestBattleOver())
        {
            return;
        }
        
        if (activeCharacterBattle == playerCharacterBattle)
        {
            SetActiveCharacter(enemyCharacterBattle);
            _state = BattleState.EnemyTurn;
            
            Debug.Log("Enemy Attack");
            //Play attack animation
            enemyCharacterBattle.BasicAttack(playerCharacterBattle, delegate
            {
                //After attack finished
                Debug.Log("Enemy attack finished");
                ChooseNextActiveCharacter();
            });
        }
        else
        {
            SetActiveCharacter(playerCharacterBattle);
            _state = BattleState.PlayerTurn;
        }
    }

    private bool TestBattleOver()
    {
        if (playerCharacterBattle.IsDefeated())
        {
            //Player dead, enemy wins
            Debug.Log("Enemy wins");
            BattleOverWindow.instance.Show(false);
            return true;
        }
        else if(enemyCharacterBattle.IsDefeated())
        {
            //Enemy dead, player wins
            Debug.Log("Player wins");
            BattleOverWindow.instance.Show(true);
            return true;
        }

        return false;
    }

    private void OnEnable()
    {
        _playerInput.CombatControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.CombatControls.Disable();
    }
}
