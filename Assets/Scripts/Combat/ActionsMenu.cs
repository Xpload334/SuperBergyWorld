using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionsMenu : MonoBehaviour
{
    public bool IsOpen;
    private BattleHandler _battleHandler;

    public GameObject layout;
    public List<GameObject> buttonObjects;
    private void Awake()
    {
        CloseMenu();
    }

    public void OpenMenu()
    {
        layout.SetActive(true);
        //On open menu, make current selected object the first
        EventSystem.current.SetSelectedGameObject(null);
        //Set new selected object
        EventSystem.current.SetSelectedGameObject(buttonObjects[0]);

        IsOpen = true;
    }

    public void CloseMenu()
    {
        layout.SetActive(false);
        IsOpen = false;
    }
}
