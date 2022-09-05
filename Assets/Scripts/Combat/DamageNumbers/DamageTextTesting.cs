using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CodeMonkey.Utils;
using Random = UnityEngine.Random;

public class DamageTextTesting : MonoBehaviour
{
    private PlayerInput _playerInput;
    private Transform _damagePopup;
    
    private void Awake()
    {
        _playerInput = new PlayerInput();
        
        _playerInput.CharacterControls.Action.started += OnAction;
        _playerInput.CharacterControls.Action.canceled += OnAction;
    }

    void OnAction(InputAction.CallbackContext ctx)
    {
        bool isCrit = Random.Range(0, 100) < 30;
        DamageText.Create(transform.position, 300, isCrit);
    }

    void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }
}
