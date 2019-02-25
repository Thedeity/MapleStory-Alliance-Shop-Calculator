using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ItemSO/Create ItemSO ")]
public class ItemSO : ScriptableObject
{
    [SerializeField]
    public string ItemName;
    public int Price;
    public int Index;

    // useless due to these fields are not editable in build
    public int UpperBound;
    public int NumOfPurchase;   
    private int totalCost;
    public int TotalCost
    {
        get
        {
            totalCost = Price * NumOfPurchase;
            return totalCost;
        }
    }
}
