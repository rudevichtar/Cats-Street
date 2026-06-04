using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private CatUIList catUIList;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool gameOver;

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void CheckGameOver()
    {
        if (gameOver)
            return;

        if (catUIList != null && catUIList.CatsCount > 0)
            return;

        EndGame();
    }

    private void EndGame()
    {
        gameOver = true;

        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (SaveLoadBridge.ShouldDeleteLoadedSlot)
            SaveSystem.DeleteSave(SaveLoadBridge.SelectedSlot);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
