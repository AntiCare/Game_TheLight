using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    [SerializeField] private MerchantInventory thingsAndStuff;
    public void DisplayShop(bool active)
    {
        UIManager.Instance.ToggleItemShop(active, thingsAndStuff.ItemsForSale);
    }
}