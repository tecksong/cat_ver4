using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void Start()
    {
        Invoke("Setup", 2);
    }

    public void Setup()
    {
        SceneManager.LoadScene(2);
    }

    public void newgame()
    {
        MainMenu.mapLevel = 1;
        MainMenu.HP = 100;
        SceneManager.LoadScene(1);
    }
    public void exit()
    {
        Application.Quit();
    }
}
