using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattleAnimations : MonoBehaviour
{
    public RuntimeAnimatorController runtimeAnimatorController;
    public GameObject spriteObject;
    // Start is called before the first frame update
    void Start()
    {
        spriteObject.GetComponent<Animator>().runtimeAnimatorController = runtimeAnimatorController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * This will potentially be expanded to play a length animation sequence
     * OnHit action will be invoked differently depending on attack minigame performance
     *
     * Will be different for enemies
     */
    public void PlayAnimAttack(Vector3 attackDirection, Action OnHit, Action OnFinished)
    {
        //Startup
        Debug.Log("Attacking towards"+attackDirection);
        
        //Target hit
        Debug.Log("Target hit");
        OnHit.Invoke();
        
        //Attack finished
        Debug.Log("Attack finished");
        OnFinished.Invoke();
        
    }

    public void PlayAnimIdle(Vector3 attackDirection)
    {
        
    }

    public void PlayAnimDefeated()
    {
        
    }
}
