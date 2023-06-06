using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public List<int> prices;
    public int buyCount;
    public int upgradeAmount;

    private TextMeshProUGUI priceText;
    void Start()
    {
        priceText = transform.Find("Price").gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (buyCount < prices.Count)
        {
            priceText.text = "$" + prices[buyCount];
        }
        else
        {
            priceText.text = "SOLD OUT";
        }
    }
    public void IncreaseBuyCount()
    {
        if (buyCount < prices.Count)
        {
            buyCount++;
        }
    }
}
