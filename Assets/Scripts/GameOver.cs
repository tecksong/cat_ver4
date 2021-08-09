using System.Collections;
using System.Collections.Generic;
using Completed;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void Start()
    {


        GameOverSoundManager.instance.musicSource.Play();
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
        GameOverSoundManager.instance.musicSource.Stop();
        SoundManager.instance.musicSource.Play();
    }
    public void exit()
    {
        Application.Quit();
    }
}
