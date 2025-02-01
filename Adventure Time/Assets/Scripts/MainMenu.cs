using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {
        Debug.Log("Игра запущена");
        SceneManager.LoadSceneAsync(1);
    }
 
    public void QuitGame()
    {
        Application.Quit();
    }
}


