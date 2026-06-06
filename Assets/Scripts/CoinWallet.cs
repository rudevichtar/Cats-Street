using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinWallet : MonoBehaviour
{
    public static CoinWallet Instance { get; private set; }

    private int coins = 100;
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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayCoinSpend();

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

    public void AddDailyReward(int day, int catsCount, int healthyCatsCount)
    {
        int baseReward = 40;

        int rewardPerDay = 10;
        int maxDayBonus = 80;

        int rewardPerCat = 10;
        int rewardPerHealthyCat = 5;

        int dayBonus = Mathf.Min((day - 1) * rewardPerDay, maxDayBonus);

        int totalReward =
            baseReward +
            dayBonus +
            catsCount * rewardPerCat +
            healthyCatsCount * rewardPerHealthyCat;

        Add(totalReward);

        Debug.Log($"Ежедневная награда: {totalReward} монет");
    }
}
