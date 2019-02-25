using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemLevel/Create ItemLevel ")]
public class ItemLevel : ScriptableObject
{
    [SerializeField]
    public List<ItemPage> Page = new List<ItemPage>();

    private int totalCost;
    public int TotalCost
    {
        get
        {
            totalCost = 0;
            foreach (var _ItemPage in Page)
            {
                totalCost += _ItemPage.TotalCost;
            }
            return totalCost;
        }
    }
}
