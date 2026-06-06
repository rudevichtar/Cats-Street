using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// UI
public class CatCard : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private TMP_Text catNameText;
    [SerializeField] private Image catIconImage;

    [Header("Sliders")]
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private Slider sleepinessSlider;
    [SerializeField] private Slider happinessSlider;
    [SerializeField] private Slider healthSlider;

    private CatNeeds catNeeds;
    private CatProfile catProfile;

    public void Init(CatNeeds needs)
    {
        if (needs == null)
        {
            Debug.LogError("CatCard.Init: CatNeeds не передан");
            return;
        }

        

        catNeeds = needs;
        catProfile = needs.GetComponent<CatProfile>();

        Debug.Log(catProfile != null
            ? $"Icon: {catProfile.CatIcon}"
            : "CatProfile не найден");

        SetupSlider(hungerSlider);
        SetupSlider(sleepinessSlider);
        SetupSlider(happinessSlider);
        SetupSlider(healthSlider);

        UpdateCard();
    }

    private void SetupSlider(Slider slider)
    {
        if (slider == null)
            return;

        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.interactable = false;
    }

    private void Update()
    {
        if (catNeeds == null)
            return;

        UpdateCard();
    }

    private void UpdateCard()
    {
        if (catNameText != null)
        {
            catNameText.text = catProfile != null
                ? catProfile.CatName
                : "Кошка";
        }

        if (catIconImage != null && catProfile != null)
            catIconImage.sprite = catProfile.CatIcon;

        hungerSlider.value = catNeeds.hunger;
        sleepinessSlider.value = catNeeds.sleepiness;
        happinessSlider.value = catNeeds.happiness;
        healthSlider.value = catNeeds.health;
    }
}
