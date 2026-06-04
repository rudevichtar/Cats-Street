using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("UI")]
    [SerializeField] private GameObject noSavePanel;

    private void Start()
    {
        if (noSavePanel != null)
            noSavePanel.SetActive(false);
    }

    public void StartNewGame()
    {
        Time.timeScale = 1f;

        SaveLoadBridge.ShouldLoadGame = false;
        SaveLoadBridge.ShouldDeleteLoadedSlot = false;

        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadGame(int slot)
    {
        if (!SaveSystem.HasSave(slot))
        {
            if (noSavePanel != null)
                noSavePanel.SetActive(true);

            return;
        }

        Time.timeScale = 1f;

        SaveLoadBridge.SelectedSlot = slot;
        SaveLoadBridge.ShouldLoadGame = true;
        SaveLoadBridge.ShouldDeleteLoadedSlot = true;

        SceneManager.LoadScene(gameSceneName);
    }
}

public static class SaveLoadBridge
{
    public static bool ShouldLoadGame;
    public static int SelectedSlot;

    public static bool ShouldDeleteLoadedSlot;
}