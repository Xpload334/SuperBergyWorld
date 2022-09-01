using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattle : MonoBehaviour
{
    public bool isPlayerTeam;
    
    [Header("State")]
    private State _state;
    
    [Header("Animations")]
    private CharacterBattleAnimations _characterBattleAnimations;
    public SpriteRenderer characterSpriteRenderer;
    public GameObject selectionCircle;
    
    private Action _onMoveComplete;
    //public Vector3 moveTargetPositionChange; //Local vector3 to add to the position
    private Vector3 _moveTargetPosition; //Actual position to move to
    
    private enum State
    {
        Idle,
        Moving,
        Busy
    }
    
    //Unit stats?
    [Header("Stats")] 
    public UnitStats unitStats;
    private HealthSystem _healthSystem;
    public GameObject healthBarGameObject;
    private HealthBar _healthBar;
    
    private void Awake()
    {
        _characterBattleAnimations = GetComponent<CharacterBattleAnimations>();
        
        HideSelectionCircle();
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Idle:
                break;
            case State.Moving:
                //Note: add a check here to make enemies not slide towards players
                
                //Move towards target position
                float moveSpeed = 4f;
                
                //Movement
                //Maybe replace with different movement method?
                transform.position += (_moveTargetPosition - GetPosition()) * (moveSpeed * Time.deltaTime);

                float reachedDistance = 0.1f;
                if (Vector3.Distance(GetPosition(), _moveTargetPosition) < reachedDistance)
                {
                    //Arrived at position
                    transform.position = _moveTargetPosition;
                    _onMoveComplete.Invoke();
                }
                break;
            case State.Busy:
                break;
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void Setup(bool isPlayerTeam, UnitStats unitStats)
    {
        //Make a reference to an animator too
        this.isPlayerTeam = isPlayerTeam;
        
        if (isPlayerTeam)
        {
            //Testing: set sprite
            //Later set the animator for this component
            characterSpriteRenderer.sprite = BattleHandler.GetInstance().playerSpriteTest;
        }
        else
        {
            characterSpriteRenderer.sprite = BattleHandler.GetInstance().enemySpriteTest;
        }
        
        //Unit stats
        this.unitStats = unitStats;
        
        //Health
        _healthSystem = new HealthSystem(unitStats.maxHealth, unitStats.health); //test
        
        //Healthbar retrieved from prefab
        _healthBar = healthBarGameObject.GetComponent<HealthBar>();
        _healthBar.Setup(_healthSystem); 
        //Animate facing left or right
        PlayAnimIdle();
    }

    private void PlayAnimIdle()
    {
        if (isPlayerTeam)
        {
            //Idle right
        }
        else
        {
            //Idle left
        }
    }

    public void Damage(CharacterBattle attacker, int amount)
    {
        Vector3 dirFromAttacker = (GetPosition() - attacker.GetPosition()).normalized;
        
        _healthSystem.Damage(amount);
        //Create damage text
        DamageText.Create(GetPosition(), amount, false, dirFromAttacker);

        if (_healthSystem.IsDefeated())
        {
            _characterBattleAnimations.PlayAnimDefeated();
        }
    }

    public void Heal(int amount)
    {
        _healthSystem.Heal(amount);
    }

    public bool IsDefeated()
    {
        return _healthSystem.IsDefeated();
    }

    public void BasicAttack(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        float distanceFromTarget = 2f;
        Vector3 targetPosition = targetCharacterBattle.GetPosition() +
                                 (GetPosition() - targetCharacterBattle.GetPosition()).normalized * distanceFromTarget;
        Vector3 startingPosition = GetPosition();
        
        //Move to target
        MoveToPosition(targetPosition, () =>
        {
            _state = State.Busy;
            Vector3 attackDirection = (targetCharacterBattle.GetPosition() - GetPosition()).normalized;
            //Play attack animation, also with the direction the target is in
            _characterBattleAnimations.PlayAnimAttack(attackDirection, () =>
            {
                //Target hit
                int damage = unitStats.basicAttack.GetComponent<AttackTarget>()
                    .CalculateDamage(unitStats, targetCharacterBattle.unitStats);
                
                targetCharacterBattle.Damage(this, damage); //test
                Debug.Log("Attacked for "+damage+" ("+targetCharacterBattle._healthSystem.GetHealth()+" HP)");
            }, () =>
            {
                //On finished
                //Move back
                MoveToPosition(startingPosition, () =>
                {
                    _state = State.Idle;
                    //Set back to idle animation
                    _characterBattleAnimations.PlayAnimIdle(attackDirection);
                    //Call attack complete
                    onAttackComplete.Invoke();
                });
            });
            
        });

    }

    private void MoveToPosition(Vector3 position, Action onMoveComplete)
    {
        this._moveTargetPosition = position;
        this._onMoveComplete = onMoveComplete;
        
        _state = State.Moving;
        //Play move animations
        if (position.x > 0)
        {
            //Moving right
        }
        else
        {
            //Moving left
        }
    }

    public void HideSelectionCircle()
    {
        selectionCircle.SetActive(false);
    }

    public void ShowSelectionCircle()
    {
        selectionCircle.SetActive(true);
    }
}
