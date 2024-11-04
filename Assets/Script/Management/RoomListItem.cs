using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class RoomListItem : MonoBehaviour
{
    public TMP_Text roomInfo;
    // Start is called before the first frame update
    public Action<string> onDelegate;
    public void SetInfo(string roomName, int currPlayer, int maxPlayer)
    {
        name = roomName;
        roomInfo.text = roomName + '('+ currPlayer +'/'+maxPlayer+')';
    }
    public void OnClick()
    {
        if(onDelegate !=null)
        {
            onDelegate(name);
        }
    }
}
