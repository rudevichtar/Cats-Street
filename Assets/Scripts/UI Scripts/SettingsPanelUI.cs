using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject tutorialPanel;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        if (musicSlider != null && AudioManager.Instance != null)
        {
            float musicVolume = AudioManager.Instance.GetMusicVolume();
            musicSlider.value = musicVolume;
            AudioManager.Instance.SetMusicVolume(musicVolume);
        }

        if (sfxSlider != null && AudioManager.Instance != null)
        {
            float sfxVolume = AudioManager.Instance.GetSfxVolume();
            sfxSlider.value = sfxVolume;
            AudioManager.Instance.SetSfxVolume(sfxVolume);
        }
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void SetMusicVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    public void SetSfxVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSfxVolume(value);
    }

    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
    }
}
