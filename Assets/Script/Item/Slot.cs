using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public int slotNum;
    public Item item;
    public Image icon;
    public Image textImage;
    public Text text;

    public void UpdateSlotUI()
    {
        icon.sprite = item.itemImage;
        icon.gameObject.SetActive(true);
    }
    public void RemoveSlot()
    {
        item = null;
        icon.gameObject.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item.itemImage != null)
        {
            textImage.gameObject.SetActive(true);
            text.text = item.itemName + ":" + ItemDatabase.instance.itemInfo[item.itemName];
        }
        else
            return;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textImage.gameObject.SetActive(false);
    }
}
