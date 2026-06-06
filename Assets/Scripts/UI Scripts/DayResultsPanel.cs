using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayResultsPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [Header("Main")]
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text catsText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text lostCatsText;
    [SerializeField] private TMP_Text difficultyText;

    [Header("Limits")]
    [SerializeField] private TMP_Text dogsText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text bedText;
    [SerializeField] private TMP_Text roadsText;
    [SerializeField] private TMP_Text nodesText;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void Show(
        int day,
        int catsCount,
        int coinsReward,
        int lostCats,
        float difficultyMultiplier,
        int maxDogs,
        int maxFoodPlacements,
        int maxBedPlacements,
        int maxBlockedRoads,
        int maxBlockedNodes)
    {
        Time.timeScale = 0f;

        if (dayText != null)
            dayText.text = $"День {day}";

        if (catsText != null)
            catsText.text = $"Количество кошек: {catsCount}";

        if (coinsText != null)
            coinsText.text = $"+{coinsReward} монет";

        if (lostCatsText != null)
            lostCatsText.text = $"Потеряно кошек: {lostCats}";

        if (difficultyText != null)
            difficultyText.text = $"Сложность: x{difficultyMultiplier:0.0}";

        if (dogsText != null)
            dogsText.text = $"Собак на карте: {maxDogs}";

        if (foodText != null)
            foodText.text = $"Миски: раз в {maxFoodPlacements} сек.";

        if (bedText != null)
            bedText.text = $"Лежанки: раз в {maxBedPlacements} сек.";

        if (roadsText != null)
            roadsText.text = $"Блокировка дорог: {maxBlockedRoads} шт";

        if (nodesText != null)
            nodesText.text = $"Блокировка точек: {maxBlockedNodes} шт";

        if (panel != null)
            panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);

        Time.timeScale = 1f;
    }
}
