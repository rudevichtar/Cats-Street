using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinWallet : MonoBehaviour
{
    // ÁÀÇÎÂÛÉ ÏÐÈÌÅÐ
   public static CoinWallet Instance { get; private set; }

    [SerializeField] private int coins;
    [SerializeField] private TMP_Text coinsText;

    public int Coins => coins;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        RefreshUI();
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0)
            return true;

        if (coins < amount)
            return false;

        coins -= amount;
        RefreshUI();
        return true;
    }

    public void Add(int amount)
    {
        if (amount <= 0)
            return;

        coins += amount;
        RefreshUI();
    }

    public void SetCoins(int value)
    {
        coins = Mathf.Max(0, value);

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (coinsText != null)
            coinsText.text = coins.ToString();
    }
}
