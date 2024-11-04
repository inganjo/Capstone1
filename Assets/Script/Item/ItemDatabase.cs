using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    #region Singleton
    public static ItemDatabase instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion
    public bool isItemGet;
    public List<Item> itemDB = new List<Item>();
    #region Dictionary
    public Dictionary<string, string> itemInfo = new Dictionary<string, string>()
    {
        
    };
    #endregion

    private void Update()
    {
        if (isItemGet)
        {
            StartCoroutine(SetBool());
        }
    }
    IEnumerator SetBool()
    {
        yield return new WaitForSeconds(0.1f);
        isItemGet = false;
    }
    
}
