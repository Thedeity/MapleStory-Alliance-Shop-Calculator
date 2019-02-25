using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopList/Create ShopList ")]
public class ShopList : ScriptableObject
{
    [SerializeField]
    public List<ShopSO> Level = new List<ShopSO>();
}
