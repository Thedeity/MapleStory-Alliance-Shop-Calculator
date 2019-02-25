using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemPage/Create ItemPage ")]
public class ItemPage : ScriptableObject
{
    [SerializeField]
    public List<ItemSO> Item = new List<ItemSO>();

    private int totalCost;
    public int TotalCost
    {
        get
        {
            totalCost = 0;
            foreach (var _ItemSO in Item)
            {
                totalCost += _ItemSO.TotalCost;
            }
            return totalCost;
        }
    }
}
