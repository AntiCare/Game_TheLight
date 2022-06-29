using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Merchant Inventory", menuName = "ScriptableObjects/Merchant Inventory")]
[Serializable]
public class MerchantInventory : ScriptableObject
{
    [SerializeField] private List<Item> itemsForSale;

    public List<Item> ItemsForSale => itemsForSale;
}