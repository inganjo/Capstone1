using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    Inventory inven;
    public Slot[] slots;
    public Transform slotHolder;

    private int currentSlot=-1;

    private void Start()
    {
        inven = GetComponentInParent<Inventory>().instance;
        slots = slotHolder.GetComponentsInChildren<Slot>();
        inven.itemChange += RedrawSlotUI;
    }

    void RedrawSlotUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].slotNum = i;
            slots[i].RemoveSlot();
        }
        for (int i = 0; i < inven.items.Count; i++)
        {
            slots[i].item = inven.items[i];
            slots[i].UpdateSlotUI();
        }
        UpdateSlotHighlight();
    }
    
    public void SetCurrentSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Length)
        {
            currentSlot = slotIndex;
            UpdateSlotHighlight();
        }
        else if (slotIndex < 0)
        {
            slots[currentSlot].ResetBorder();
            currentSlot=-1;
        }
    }

    private void UpdateSlotHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i == currentSlot)
            {
                slots[i].HighlightSlot();
            }
            else
            {
                slots[i].ResetBorder();
            }
        }
    }
}
