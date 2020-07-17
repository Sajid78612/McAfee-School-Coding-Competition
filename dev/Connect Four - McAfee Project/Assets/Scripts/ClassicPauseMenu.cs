using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClassicPauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        GameIsPaused = false;
    }

    public void RestartTimer()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        AudioListener.pause = false;
        SceneManager.LoadScene("PVPTimer");
    }

    public void RestartNormal()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        AudioListener.pause = false;
        SceneManager.LoadScene("PlayerVsPlayerLOCAL");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
        GameIsPaused = true;
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        AudioListener.pause = false;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}