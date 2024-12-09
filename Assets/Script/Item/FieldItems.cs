using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FieldItems : MonoBehaviour
{
    public Item item;
    public SpriteRenderer image;
    
    public void SetItem(Item _item){
        item.itemName=_item.itemName;
        item.itemImage=_item.itemImage;
        item.itemType=_item.itemType;

        image.sprite=item.itemImage;
    }

    public Item GetItem()
    {
        return item;
    }

    public void DestroyItem()
    {
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else{
            Destroy(gameObject);
        }

    }
}
