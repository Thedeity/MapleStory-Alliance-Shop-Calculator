using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemList/Create ItemList ")]
public class ItemList : ScriptableObject
{
    [SerializeField]
    public List<ItemLevel> Level = new List<ItemLevel>();

    private int totalCost;
    public int TotalCost
    {
        get
        {
            totalCost = 0;
            foreach (var _ItemLevel in Level)
            {
                totalCost += _ItemLevel.TotalCost;
            }
            return totalCost;
        }
    }
}
