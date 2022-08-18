using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item", order = 0)]
[System.Serializable]
public class Item : ScriptableObject 
{
    public Sprite itemSprite;
    public string itemDescription;
    public bool isKey;
    public bool isCoin;
    public int coins;
}
