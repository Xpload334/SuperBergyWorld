using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EnemySelectButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private CharacterBattle _characterBattle; //Character battle to target
    [SerializeField] private TMP_Text tmpText;

    public CharacterBattle CharacterBattle
    {
        get { return _characterBattle; }
        set { _characterBattle = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        CharacterBattle.ShowSelectionCircle();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        CharacterBattle.HideSelectionCircle();
    }

    public void SetText(string text)
    {
        tmpText.text = text;
    }
}
