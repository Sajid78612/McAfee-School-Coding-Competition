using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArcadePauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    public Button btnBlackhole;
    public Button btnExtraGo;

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
        btnBlackhole.interactable = !btnBlackhole.interactable;
        btnExtraGo.interactable = !btnExtraGo.interactable;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        AudioListener.pause = false;
        SceneManager.LoadScene("ArcadePVPLocal");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;
        GameIsPaused = true;
        btnBlackhole.interactable = !btnBlackhole.interactable;
        btnExtraGo.interactable = !btnExtraGo.interactable;
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