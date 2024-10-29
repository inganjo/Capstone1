using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
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
    public bool AddItem(Item _item)
    {
        if (items.Count < slotAmount)
        {
            items.Add(_item);
            if (itemChange!=null)
                itemChange.Invoke();
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
