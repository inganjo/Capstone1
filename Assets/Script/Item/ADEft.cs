using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Equipment/AD")]
public class ADEft : ItemEffect
{
    public float ADPoint;
    public override bool ExecuteRole()
    {
        return true;
    }
}
