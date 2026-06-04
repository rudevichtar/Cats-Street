using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] private int slotNumber;

    [Header("States")]
    [SerializeField] private GameObject emptyState;
    [SerializeField] private GameObject filledState;

    [Header("Empty UI")]
    [SerializeField] private TMP_Text emptyTitleText;

    [Header("Filled UI")]
    [SerializeField] private TMP_Text filledTitleText;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Image saveIconImage;

    [Header("Icons")]
    [SerializeField] private Sprite filledSlotIcon;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (!SaveSystem.HasSave(slotNumber))
        {
            ShowEmpty();
            return;
        }

        SaveData data = SaveSystem.LoadPreview(slotNumber);

        if (data == null)
        {
            ShowEmpty();
            return;
        }

        ShowFilled(data);
    }

    private void ShowEmpty()
    {
        if (emptyState != null)
            emptyState.SetActive(true);

        if (filledState != null)
            filledState.SetActive(false);

        if (emptyTitleText != null)
            emptyTitleText.text = $"Слот {slotNumber}";
    }

    private void ShowFilled(SaveData data)
    {
        if (emptyState != null)
            emptyState.SetActive(false);

        if (filledState != null)
            filledState.SetActive(true);

        if (filledTitleText != null)
            filledTitleText.text = $"Слот {slotNumber}";

        if (infoText != null)
        {
            infoText.text =
                $"Дней: {data.days}\n" +
                $"Котов: {CountAliveCats(data)}\n" +
                $"Время: {FormatTime(data.totalGameTime)}\n" +
                $"Монеты: {data.coins}\t\t\t\t" +
                $"Дата: {data.saveDate}";
        }

        if (saveIconImage != null)
            saveIconImage.sprite = filledSlotIcon;
    }

    private int CountAliveCats(SaveData data)
    {
        int count = 0;

        foreach (CatSaveData cat in data.cats)
        {
            if (cat.health > 0f)
                count++;
        }

        return count;
    }

    private string FormatTime(float seconds)
    {
        int hours = Mathf.FloorToInt(seconds / 3600f);
        int minutes = Mathf.FloorToInt((seconds % 3600f) / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);

        return $"{hours:00}:{minutes:00}:{secs:00}";
    }
}
