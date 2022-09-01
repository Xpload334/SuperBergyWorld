using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CombatInput
{
    None,
    Primary,
    Secondary,
    Previous,
    Next,
    Up,
    Down,
    Left,
    Right
}

public enum AttackStrengths
{
    Miss,
    Good,
    Perfect
}

public class AttackMinigameInputHandler : MonoBehaviour
{
    public bool IsWrongInput; //Input is incorrect
    public bool IsGoodDamage; //Animation currently in good damage zone
    public bool IsPerfectDamage; //Animation currently in perfect damage zone
    private PlayerInput _playerInput;
    public CombatInput currentCombatInput; //Input provided by the player
    public CombatInput inputToCheckFor; //Input to test for
    private void Awake()
    {
        _playerInput = new PlayerInput();

        currentCombatInput = CombatInput.None; //Set to none initially
        inputToCheckFor = CombatInput.None;
        
        //Primary
        _playerInput.CombatControls.Primary.started += _ => currentCombatInput = CombatInput.Primary;
        _playerInput.CombatControls.Primary.canceled += _ => currentCombatInput = CombatInput.None; //On release set input to none
        
        _playerInput.CombatControls.Secondary.started += _ => currentCombatInput = CombatInput.Secondary;
        _playerInput.CombatControls.Secondary.canceled += _ => currentCombatInput = CombatInput.None;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inputToCheckFor != CombatInput.None)
        {
            //While currently testing input
            //If perfect zone and input correct
            if (IsPerfectDamage && currentCombatInput == inputToCheckFor)
            {
                //Perfect hit
                HitPerfect();
            }
            //if good zone and input correct
            else if (IsGoodDamage && currentCombatInput == inputToCheckFor)
            {
                //Good hit
                HitGood();
            }
            //If any other input, apart from none
            else if (currentCombatInput != CombatInput.None)
            {
                //Fail attack animation
                FailAttack();
            }
            //On none, do nothing
        }
    }

    //Below events take place in chronological order
    //Perfect bool is prioritised over good timing
    
    /*
     * Enable checking for the desired input
     */
    public void EnableInputCheck(CombatInput correctInput)
    {
        inputToCheckFor = correctInput;
    }
    
    public void EnableGood()
    {
        IsGoodDamage = true;
    }

    public void EnablePerfect()
    {
        IsPerfectDamage = true;
    }

    public void DisablePerfect()
    {
        IsPerfectDamage = false;
    }

    public void DisableGood()
    {
        IsGoodDamage = false;
    }

    public void DisableInputCheck()
    {
        inputToCheckFor = CombatInput.None;
    }

    public void FailAttack()
    {
        
    }

    //Communicate with battle handler
    public void HitGood()
    {
        
    }

    public void HitPerfect()
    {
        
    }
    
    
}
