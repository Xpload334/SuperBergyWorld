using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEncounter : MonoBehaviour
{
    public int experienceReward;

    public int dialogueIDOnStart;
    

    public void CollectReward()
    {
        Destroy(this.gameObject);
    }
}
