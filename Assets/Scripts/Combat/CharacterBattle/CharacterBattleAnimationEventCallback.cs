using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBattleAnimationEventCallback : MonoBehaviour
{
    public CharacterBattleAnimations battleAnimations;
    
    public void OnHit()
    {
        battleAnimations.InvokeOnHitAction();
    }

    public void OnFinished()
    {
        battleAnimations.InvokeOnFinishedAction();
    }
}
