using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public List<GameObject> shopPages;
    public GameManager gameManager;
    public PlayerController playerController;

    public List<int> upgradeAmounts;
    public List<ShopItem> shopItems;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void OpenShopPage(int pageNumber)
    {
        foreach (GameObject page in shopPages)
        {
            page.gameObject.SetActive(false);
        }
        shopPages[pageNumber].gameObject.SetActive(true);
    }
    public void BuyUpgrade(ShopItem shopItem)
    {
        if (shopItem.buyCount < shopItem.prices.Count)
        {
            if (gameManager.currency < shopItem.prices[shopItem.buyCount])
            {
                return;
            }
            else
            {
                gameManager.currency -= shopItem.prices[shopItem.buyCount];
                shopItem.IncreaseBuyCount();
            }
            switch (shopItem.name)
            {
                case "Jump Power":
                    playerController.jumpForce += shopItem.upgradeAmount;
                    break;
                case "Jump Count":
                    playerController.extraJumps += shopItem.upgradeAmount; 
                    break;
                case "Launch Power":
                    playerController.launchForce += shopItem.upgradeAmount;
                    break;
                case "Launch Count":
                    playerController.extraLaunches += shopItem.upgradeAmount;
                    break;
                case "Currency Multiplier":
                    gameManager.currencyMultiplier += (float)shopItem.upgradeAmount / 100;
                    break;
                case "Enemy Count":
                    gameManager.levelThresholdDistance += shopItem.upgradeAmount;
                    break;
            }
        }
    }
}
