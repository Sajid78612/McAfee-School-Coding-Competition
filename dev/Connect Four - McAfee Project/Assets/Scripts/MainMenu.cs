using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void PlayervsPlayer()
    {
        SceneManager.LoadScene("PlayerVsPlayerLOCAL");
    }

    public void PvPTimer()
    {
        SceneManager.LoadScene("PvPTimer");
    }

    public void ArcadePvPLocal()
    {
        SceneManager.LoadScene("ArcadePVPLocal");
    }

    public void ArcadePVPTimer()
    {
        SceneManager.LoadScene("ArcadePVPTimer");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
