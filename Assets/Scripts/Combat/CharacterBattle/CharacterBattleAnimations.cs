using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattleAnimations : MonoBehaviour
{
    public RuntimeAnimatorController runtimeAnimatorController;
    public GameObject spriteObject;
    private Animator _animator;
    
    private int _moveXHash;
    private int _moveYHash;

    private int _attackHash;

    [Header("Actions")] 
    public bool shouldInvokeActions = true;
    private Action _onHitAction;
    private Action _onFinishedAction;
    // Start is called before the first frame update
    void Start()
    {
        spriteObject.GetComponent<Animator>().runtimeAnimatorController = runtimeAnimatorController;
        _animator = spriteObject.GetComponent<Animator>();
        
        _moveXHash = Animator.StringToHash("MoveX");
        _moveYHash = Animator.StringToHash("MoveY");
        _attackHash = Animator.StringToHash("Attack");
    }

    /*
     * This will potentially be expanded to play a length animation sequence
     * OnHit action will be invoked differently depending on attack minigame performance
     *
     * Will be different for enemies
     */
    public void PlayAnimAttack(Vector3 attackDirection, Action onHit, Action onFinished)
    {
        //Startup
        Debug.Log("Attacking towards"+attackDirection);
        _animator.SetFloat(_moveXHash, attackDirection.x);
        _animator.SetFloat(_moveYHash, attackDirection.z);
        
        //Target hit
        _animator.SetTrigger(_attackHash);
        _onHitAction = onHit;
        
        //Attack finished
        _onFinishedAction = onFinished;
    }

    public void PlayAnimIdle(Vector3 attackDirection)
    {
        
    }

    public void PlayAnimDefeated()
    {
        
    }

    /*
     * Method called from Animation Events
     */
    public void InvokeOnHitAction()
    {
        if (shouldInvokeActions)
        {
            Debug.Log("Target hit");
            _onHitAction.Invoke();
        }
    }

    public void InvokeOnFinishedAction()
    {
        if (shouldInvokeActions)
        {
            Debug.Log("Attack finished");
            _onFinishedAction.Invoke();
        }
    }
    
    
    
}
