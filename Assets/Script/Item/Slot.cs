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
    public Image border;

    public void UpdateSlotUI()
    {
        if (item != null)
        {
            icon.sprite = item.itemImage;
            icon.gameObject.SetActive(true);
        }
        else
        {
            RemoveSlot();
        }
    }
    public void RemoveSlot()
    {
        item = null;
        icon.gameObject.SetActive(false);
        ResetBorder();
    }

    public void HighlightSlot()
    {
        if (border != null)
        {
            border.color=Color.white;
            border.enabled=true;
        }
    }

    public void ResetBorder()
    {
        if (border!= null)
        {
            border.color=Color.clear;
            border.enabled=false;
        }
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
