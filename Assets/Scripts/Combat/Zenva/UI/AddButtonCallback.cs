using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Since player party is not in the same scene as the combat buttons, this adds the callback in Start()
 */
public class AddButtonCallback : MonoBehaviour
{
    [SerializeField]
    private bool isBasicAttack;
    // Use this for initialization
    void Start () 
    {
        this.gameObject.GetComponent<Button>().onClick.AddListener(() => AddCallback());
    }
    private void AddCallback ()
    {
        GameObject playerParty = FindObjectOfType<IPlayerParty>().gameObject;
        playerParty.GetComponent<SelectUnit>().selectAttack(this.isBasicAttack);
    }
}
