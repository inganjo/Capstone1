using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
public class Inventory : MonoBehaviourPunCallbacks
{

    public Inventory instance;
    private void Awake()
    {
        instance = this;
    }

    public delegate void ItemChange();
    public ItemChange itemChange;
    public int slotAmount = 4;
    public List<Item> items = new List<Item>();
    // public PhotonHashTable playersetting = PhotonNetwork.LocalPlayer.CustomProperties;


    // void Start()
    // {
    //     if(PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("ITEM"))
    //     {
    //         items = new List<Item>((Item[])playersetting["ITEM"]).ToList();
    //     }
    // }
    public bool AddItem(Item _item)
    {
        if (items.Count < slotAmount)
        {
            items.Add(_item);
            if (itemChange!=null)
                itemChange.Invoke();
            // playersetting["ITEM"] = items.ToArray();
            // Debug.Log("인벤토리" + playersetting["ITEM"]);
            // PhotonNetwork.LocalPlayer.SetCustomProperties(playersetting);
            return true;
        }
        return false;
    }
    public void RemoveItem(int num)
    {
        items.RemoveAt(num);
        itemChange.Invoke();
    }

    private void OnTriggerStay(Collider other){
        if (other.gameObject.tag=="Item"){
            Debug.Log("Trigger");
            if (Input.GetKeyDown(KeyCode.E)) {
                FieldItems fieldItems=other.GetComponent<FieldItems>();
                if (AddItem(fieldItems.GetItem())) {
                    fieldItems.DestroyItem();
                }
            } 
        }
    }
}
