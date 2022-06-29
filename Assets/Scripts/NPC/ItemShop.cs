using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour
{
    [SerializeField] private Transform   _itemShopContent;
    [SerializeField] private Transform   _itemSellContent;
    [SerializeField] private GameObject  itemShopPrefab;
    [SerializeField] private GameObject  Shop;
    [SerializeField] private GameObject  Sell;
    [SerializeField] private GameObject  buttons;
    [SerializeField] private TMP_Text    playerWalletAmountShop;
    [SerializeField] private TMP_Text    playerWalletAmountSell;
    [SerializeField] private Button      toBuy;
    [SerializeField] private Button      toSell;
    private                  AudioSource audioSource;
    public                   AudioClip   coinClip;
    public                   AudioClip   upgradeClip;

    private Dictionary<int, int> currentInventory = new Dictionary<int, int>();
    private List<Item>           itemsForSale;

    private readonly List<GameObject> instantiatedItems = new List<GameObject>();

    enum ShopNavigation
    {
        BUY,
        SELL,
    }

    private void Start()
    {
        ToggleShop(false, null);

        toBuy.onClick.AddListener(() => { HandleTo(ShopNavigation.BUY); });
        toSell.onClick.AddListener(() => { HandleTo(ShopNavigation.SELL); });
        audioSource = GetComponent<AudioSource>();
    }

    void HandleTo(ShopNavigation shopNavigation)
    {
        toBuy.GetComponent<Image>().color  = Color.gray;
        toSell.GetComponent<Image>().color = Color.gray;

        switch (shopNavigation)
        {
            case ShopNavigation.BUY:
                toBuy.GetComponent<Image>().color = Color.white;
                Sell.SetActive(false);
                ShowShop();
                break;
            case ShopNavigation.SELL:
                toSell.GetComponent<Image>().color = Color.white;
                Sell.SetActive(true);
                ShowSell();
                break;
        }
    }

    public void ToggleShop(bool active, List<Item> items)
    {
        toBuy.GetComponent<Image>().color  = Color.white;
        toSell.GetComponent<Image>().color = Color.gray;

        itemsForSale = items;
        Shop.SetActive(active);
        buttons.SetActive(active);

        if (Shop.activeSelf)
        {
            Sell.SetActive(false);
        }

        if (active)
        {
            ShowShop();
        }
    }

    Dictionary<int, int> GetCountForItems(List<Item> items)
    {
        Dictionary<int, int> counts = new Dictionary<int, int>();

        foreach (Item item in items)
        {
            int count = 0;

            if (item.ID != 0)
            {
                foreach (Item itemCount in items)
                {
                    if (counts.ContainsKey(item.ID) && item.ID == itemCount.ID)
                    {
                        count++;
                        counts[item.ID] = count;
                    }

                    if (item.ID == itemCount.ID && !counts.ContainsKey(item.ID))
                    {
                        count++;
                        counts.Add(item.ID, count);
                    }
                }
            }
        }

        return counts;
    }

    public void ShowShop()
    {
        playerWalletAmountShop.text = GameManager.Instance.Player.PlayerWallet.ToString();
        List<Item> items = GameManager.Instance.Player.Inventory.Items;

        if (_itemShopContent.childCount > 0)
        {
            DestroyInstantiatedNoteItems();
        }

        foreach (Item item in itemsForSale)
        {
            if(item.Upgrade && items.Find(i => i.ID == item.ID)) return;

            PopulateShopInventory(item, null, _itemShopContent);
        }
    }

    public void ShowSell()
    {
        playerWalletAmountSell.text = GameManager.Instance.Player.PlayerWallet.ToString();

        if (_itemSellContent.childCount > 0)
        {
            DestroyInstantiatedNoteItems();
        }

        currentInventory = GetCountForItems(GameManager.Instance.Player.Inventory.Items);

        foreach (KeyValuePair<int, int> pair in currentInventory)
        {
            Item item = GameManager.Instance.GetItemById(pair.Key);

            if (!item.Upgrade)
            {
                PopulateShopInventory(item, pair.Value, _itemSellContent);
            }
        }
    }

    void PopulateShopInventory(Item item, int ? inventoryAmount, Transform content)
    {
        if (item.ID == 0)
        {
            return;
        }

        GameObject shopItem = Instantiate(itemShopPrefab, content, true);

        shopItem.transform.GetChild(0).GetComponent<Image>().sprite = item.ItemSprite;

        Loot lootItem = shopItem.transform.GetChild(0).GetComponent<Loot>();

        lootItem.Item = item;

        Button   plus                  = shopItem.transform.GetChild(1).GetComponent<Button>();
        Button   minus                 = shopItem.transform.GetChild(2).GetComponent<Button>();
        Button   buy                   = shopItem.transform.GetChild(3).GetComponent<Button>();
        TMP_Text amount                = shopItem.transform.GetChild(4).GetComponent<TMP_Text>();
        TMP_Text price                 = shopItem.transform.GetChild(5).GetComponent<TMP_Text>();
        TMP_Text playerInventoryAmount = shopItem.transform.GetChild(6).GetComponent<TMP_Text>();
        Button   sell                  = shopItem.transform.GetChild(7).GetComponent<Button>();

        amount.text                = "1";
        playerInventoryAmount.text = String.Empty;
        price.text                 = item.Price.ToString();

        sell.gameObject.SetActive(false);
        buy.gameObject.SetActive(true);

        if (Sell.activeSelf)
        {
            price.text                 = item.SellPrice.ToString();
            playerInventoryAmount.text = inventoryAmount.ToString();
            sell.gameObject.SetActive(true);
            buy.gameObject.SetActive(false);
        }

        sell.onClick.AddListener(() => { HandleSellButton(item, lootItem, amount); });
        plus.onClick.AddListener(() => { HandlePlusButton(item, amount); });
        minus.onClick.AddListener(() => { HandleMinusButton(item, amount); });
        buy.onClick.AddListener(() => { HandleBuyButton(item, lootItem, amount, shopItem); });

        instantiatedItems.Add(shopItem);

        shopItem.transform.localScale = Vector2.one;
    }

    void HandlePlusButton(Item item, TMP_Text amount)
    {
        if (item.Upgrade)
        {
            return;
        }

        if (int.Parse(amount.text) < 999)
        {
            int amountToBuy = int.Parse(amount.text);

            amountToBuy++;

            amount.text = amountToBuy.ToString();
        }
    }

    void HandleMinusButton(Item item, TMP_Text amount)
    {
        if (item.Upgrade)
        {
            return;
        }

        if (int.Parse(amount.text) == 0)
        {
            return;
        }

        int amountToBuy = int.Parse(amount.text);

        amountToBuy--;

        amount.text = amountToBuy.ToString();
    }

    void HandleBuyButton(Item item, Loot loot, TMP_Text amount, GameObject shopItem)
    {
        int    buyAmount = int.Parse(amount.text);
        Player player    = GameManager.Instance.Player;
        int    Wallet    = player.PlayerWallet;

        if ((buyAmount * item.Price) <= Wallet)
        {
            player.PlayerWallet = Wallet - buyAmount * item.Price;

            playerWalletAmountShop.text = player.PlayerWallet.ToString();

            if (item.Upgrade)
            {
                player.ConsumeUpgrade(item);
                Destroy(shopItem);
                PlayUpgradeClip();
                player.AddToInventory(loot);
                return;
            }

            for (int i = 0; i < buyAmount; i++)
            {
                player.AddToInventory(loot);
            }
        }
    }

    void HandleSellButton(Item item, Loot loot, TMP_Text amount)
    {
        int    sellAmount = int.Parse(amount.text);
        Player player     = GameManager.Instance.Player;

        if (sellAmount <= currentInventory[item.ID])
        {
            player.PlayerWallet += sellAmount * item.SellPrice;

            for (int i = 0; i < sellAmount; i++)
            {
                player.RemoveFromInventory(item);
            }

            UIManager.Instance.HeadsUpDisplay.BindInventory(player.Inventory.Items);
            playerWalletAmountSell.text = player.PlayerWallet.ToString();
        }

        PlayCoinClip();
        ShowSell();
    }

    void PlayCoinClip()
    {
        audioSource.clip = coinClip;
        if (audioSource.isPlaying)
            return;

        audioSource.Play();
    }

    void PlayUpgradeClip()
    {
        audioSource.clip = upgradeClip;
        if (audioSource.isPlaying)
            return;

        audioSource.Play();
    }

    void DestroyInstantiatedNoteItems()
    {
        _itemShopContent.DetachChildren();
        foreach (var obj in instantiatedItems)
        {
            Destroy(obj);
        }
    }
}