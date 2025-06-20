using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject continiueButton;
    PlayerData pd;
    private void Awake()
    {
        ClearDontDestroyOnLoad();
        GameObject go = new GameObject("SaveSystem");
        go.AddComponent<SaveSystem>();
        pd = SaveSystem.Instance.LoadGame();
        if (pd.canUse)
            continiueButton.SetActive(true);
    }
    public void PlayGame()
    {
        Debug.Log("Игра запущена");
        pd.canUse = false;
        SaveSystem.Instance.SaveGame(pd);
        SceneManager.LoadSceneAsync(1);
    }

    public void ContiniueGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
 
    public void QuitGame()
    {
        Application.Quit();
    }

    void ClearDontDestroyOnLoad()
    {
        foreach (var obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.scene.name == null || obj.scene.name == "DontDestroyOnLoad")
            {
                Destroy(obj);
            }
        }
    }
}


