using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionsMenu : MonoBehaviour
{
    public bool isOpen;
    public bool waitForCloseBeforeRemovingButtons = true;
    public Animator animator;
    public BattleHandler battleHandler;

    public GameObject layout;
    public List<GameObject> buttonObjects;
    
    private static readonly int Open = Animator.StringToHash("IsOpen");
    private void Awake()
    {
        CloseMenu();
    }

    public void OpenMenu()
    {
        //layout.SetActive(true);
        //On open menu, make current selected object the first
        SetCurrentSelectionNull();
        //Set new selected object
        EventSystem.current.SetSelectedGameObject(buttonObjects[0]);

        if (animator != null)
        {
            animator.SetBool(Open, true);
        }
        
        isOpen = true;
    }

    public void CloseMenu()
    {
        if (animator != null)
        {
            animator.SetBool(Open, false);
        }
        //layout.SetActive(false);
        isOpen = false;
    }
    
    protected void ClearLayout()
    {
        foreach (Transform child in layout.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    protected void ClearButtonsList()
    {
        buttonObjects.Clear();
    }

    protected void SetCurrentSelectionNull()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
