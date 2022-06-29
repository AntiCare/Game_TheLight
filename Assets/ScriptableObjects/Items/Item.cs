using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
[Serializable]
public class Item : ScriptableObject
{
    [SerializeField] private int id;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private int price;
    [SerializeField] private int sellPrice;
    [SerializeField] private float itemWeight;
    [SerializeField] private GameObject item;
    [SerializeField] private bool canUse;
    [SerializeField] private bool canRemove;
    [SerializeField] private bool questItem;
    [SerializeField] private bool upgrade;
    [SerializeField] private float consumeAmount;
    [SerializeField] private string itemDescription;

    public int ID => id;
    
    public string ItemName => itemName;
    
    public Sprite ItemSprite => itemSprite;
    
    public int Price => price;
    
    public int SellPrice => sellPrice;
    
    public float ItemWeight => itemWeight;
    
    public GameObject ItemObject => item;
    
    public bool CanUse => canUse;
    
    public bool CanRemove => canRemove;
    
    public bool QuestItem => questItem;
    public bool Upgrade => upgrade;
    
    public float ConsumeAmount => consumeAmount;

    public string ItemDescription => itemDescription;
}