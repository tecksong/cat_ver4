using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static int mapLevel;
    public static int HP;

    public void newGame()
    {
        mapLevel = 1;
        HP = 100;

        SceneManager.LoadScene(1);
        mainMenuSoundManager.instance.musicSource.Stop();
    }
    public void exit()
    {
        Application.Quit();
    }
}
