using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light sunLight;

    [SerializeField] private float phaseDuration = 300f;
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

    public bool IsDay => isDay;
    public int Days => days;
    public float Timer => timer;

    private void Start()
    {
        timer = phaseDuration;
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
        timer = phaseDuration;

        if (isDay)
            days++;
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
    }
}
