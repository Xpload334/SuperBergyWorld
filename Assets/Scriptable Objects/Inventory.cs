using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Items/Inventory", order = 0)]
[System.Serializable]
public class Inventory : ScriptableObject 
{
    public Item currentItem;
    public List<Item> items = new List<Item>();
    public int numberOfKeys;
    public int coins;
    
    //public SFXManager sfxManager;


    public void AddItem(Item itemToAdd)
    {
        //Is item a key
        if(itemToAdd.isKey)
        {
            numberOfKeys ++;
        }
        else if(itemToAdd.isCoin)
        {
            coins += currentItem.coins;
        }
        else
        {
            if(!items.Contains(itemToAdd))
            {
                items.Add(itemToAdd);
            }
        }
    }



}
