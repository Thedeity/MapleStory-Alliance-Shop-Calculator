using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShopSO/Create ShopSO ")]
public class ShopSO : ScriptableObject
{
    [SerializeField]
    public List<Sprite> Sprite_List = new List<Sprite>();
}