using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    OneHand,
    TwoHand
}
[System.Serializable]
public class Item
{
    public ItemType itemType;
    public string itemName;
    public Sprite itemImage;
    public GameObject itemPrefab;
    public List<ItemEffect> efts;

    public bool Use()
    {
        bool isUsed = false;
        foreach(ItemEffect eft in efts)
        {
            isUsed = eft.ExecuteRole();
        }
        return isUsed;
    }
}
