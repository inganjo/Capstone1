using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemManager : MonoBehaviour
{
    public Transform goal;
    public GameObject fieldItemPreFab;
    public int itemCount;
    [SerializeField]
    bool isNeedless;
    public Vector3[] pos;
    List<Item> itemDB; 
    void Awake()
    {
        
    }
    public void GenerateItem()
    {
        if (!isNeedless)
        {
            itemDB = ItemDatabase.instance.itemDB;
            for (int i = 0; i < 3; i++)
            {
                GameObject go = Instantiate(fieldItemPreFab, pos[i], Quaternion.identity, goal);
                //go.GetComponent<FieldItem>().SetItem(itemDB[Random.Range(0, 12)]);
            }
        }
    }
}
