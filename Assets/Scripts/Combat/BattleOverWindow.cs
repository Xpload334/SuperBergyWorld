using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BattleOverWindow : MonoBehaviour
{
    public static BattleOverWindow Instance;

    [SerializeField] private GameObject playerWinTextObject;
    [SerializeField] private GameObject playerLoseTextObject;

    private void Awake()
    {
        Instance = this;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(bool playerWin)
    {
        gameObject.SetActive(true);
        
        playerWinTextObject.SetActive(playerWin);
        playerLoseTextObject.SetActive(!playerWin);
        
    }
}
