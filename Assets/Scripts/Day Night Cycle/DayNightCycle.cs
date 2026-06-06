using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light sunLight;

    [SerializeField] private DayResultsPanel dayResultPanel;
    [SerializeField] private CatResourcePlacer catResourcePlacer;

    [Header("Cycle")]
    [SerializeField] private float dayDuration = 300f;     // 5 минут
    private float nightDuration;   // 2.5 минуты
    [SerializeField] private float transitionSpeed = 1f;

    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private Color nightColor = new Color(0.05f, 0.15f, 0.3f);

    [SerializeField] private float dayIntensity = 1f;
    [SerializeField] private float nightIntensity = 0.08f;

    [SerializeField] private TMP_Text phaseText;
    [SerializeField] private TMP_Text daysText;
    [SerializeField] private TMP_Text timerText;

    private bool isDay = true;
    private int days = 1;
    private float timer;
    private int lostCatsTotal;

    public bool IsDay => isDay;
    public int Days => days;
    public float Timer => timer;

    private void Start()
    {
        nightDuration = dayDuration / 2;

        if (BlockLimitManager.Instance != null)
            BlockLimitManager.Instance.SetDay(days);

        timer = dayDuration;
        RenderSettings.ambientMode = AmbientMode.Flat;
        UpdateUI();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
            ChangePhase();

        SmoothLightChange();
        UpdateUI();
    }

    private void ChangePhase()
    {
        isDay = !isDay;
        timer = isDay
            ? dayDuration
            : nightDuration;

        if (isDay)
        {
            days++;

            int reward = CalculateDailyReward();

            if (CoinWallet.Instance != null)
                CoinWallet.Instance.Add(reward);

            if (dayResultPanel != null)
            {
                dayResultPanel.Show(
                    days,
                    GetCatsCount(),
                    reward,
                    lostCatsTotal,
                    GetDifficultyMultiplier(),
                    GetMaxDogs(),
                    catResourcePlacer != null ? catResourcePlacer.foodCooldownSeconds : 0,
                    catResourcePlacer != null ? catResourcePlacer.bedCooldownSeconds : 0,
                    BlockLimitManager.Instance != null ? BlockLimitManager.Instance.MaxRoads : 0,
                    BlockLimitManager.Instance != null ? BlockLimitManager.Instance.MaxNodes : 0
                );
            }

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayNewDaySound();

            if (BlockLimitManager.Instance != null)
                BlockLimitManager.Instance.SetDay(days);

            GiveDailyReward();

            CarSpawner[] spawners = FindObjectsByType<CarSpawner>(FindObjectsSortMode.None);

            foreach (CarSpawner spawner in spawners)
            {
                if (spawner != null)
                    spawner.ApplyTrafficDifficulty(days);
            }

            DogDangerSystem dogSystem = FindObjectOfType<DogDangerSystem>();

            if (dogSystem != null)
                dogSystem.ApplyDogDifficulty(days);
        }
    }

    private void SmoothLightChange()
    {
        if (sunLight == null)
            return;

        Color targetColor = isDay ? dayColor : nightColor;
        float targetIntensity = isDay ? dayIntensity : nightIntensity;

        sunLight.color = Color.Lerp(
            sunLight.color,
            targetColor,
            transitionSpeed * Time.deltaTime
        );

        sunLight.intensity = Mathf.Lerp(
            sunLight.intensity,
            targetIntensity,
            transitionSpeed * Time.deltaTime
        );

        RenderSettings.ambientLight = Color.Lerp(
            RenderSettings.ambientLight,
            targetColor,
            transitionSpeed * Time.deltaTime
        );
    }

    private void UpdateUI()
    {
        if (phaseText != null)
            phaseText.text = isDay ? "День" : "Ночь";

        if (daysText != null)
            daysText.text = $"День: {days}";

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void LoadState(bool loadedIsDay, int loadedDays, float loadedTimer)
    {
        isDay = loadedIsDay;
        days = loadedDays;
        timer = loadedTimer;

        UpdateUI();

        if (BlockLimitManager.Instance != null)
            BlockLimitManager.Instance.SetDay(days);
    }

    private void GiveDailyReward()
    {
        CatNeeds[] cats = FindObjectsByType<CatNeeds>(FindObjectsSortMode.None);

        int catsCount = 0;
        int healthyCatsCount = 0;

        foreach (CatNeeds cat in cats)
        {
            if (cat == null || cat.IsDead)
                continue;

            catsCount++;

            if (cat.health >= 70f
                && cat.hunger < 70f
                && cat.sleepiness < 70f
                && cat.happiness >= 40f)
            {
                healthyCatsCount++;
            }
        }

        CoinWallet.Instance.AddDailyReward(
            days,
            catsCount,
            healthyCatsCount
        );
    }

    public void RegisterLostCat()
    {
        lostCatsTotal++;
    }

    private int GetCatsCount()
    {
        CatNeeds[] cats = FindObjectsByType<CatNeeds>(FindObjectsSortMode.None);

        int count = 0;

        foreach (CatNeeds cat in cats)
        {
            if (cat != null && !cat.IsDead)
                count++;
        }

        return count;
    }

    private int CalculateDailyReward()
    {
        CatNeeds[] cats = FindObjectsByType<CatNeeds>(FindObjectsSortMode.None);

        int catsCount = 0;
        int healthyCatsCount = 0;

        foreach (CatNeeds cat in cats)
        {
            if (cat == null || cat.IsDead)
                continue;

            catsCount++;

            if (cat.health >= 70f &&
                cat.hunger < 70f &&
                cat.sleepiness < 70f &&
                cat.happiness >= 40f)
            {
                healthyCatsCount++;
            }
        }

        int baseReward = 40;
        int rewardPerDay = 10;
        int maxDayBonus = 80;
        int rewardPerCat = 10;
        int rewardPerHealthyCat = 5;

        int dayBonus = Mathf.Min((days - 1) * rewardPerDay, maxDayBonus);

        return baseReward +
               dayBonus +
               catsCount * rewardPerCat +
               healthyCatsCount * rewardPerHealthyCat;
    }

    private float GetDifficultyMultiplier()
    {
        return 1f + (days - 1) * 0.25f;
    }

    private int GetMaxDogs()
    {
        return Mathf.Clamp(days / 2, 0, 5);
    }
}
