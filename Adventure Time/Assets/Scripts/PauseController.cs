using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public GameObject[] choices;
    public int currentChoiceIndex;
    public bool menuActive;

    public GameObject menuUI;
    public GameObject[] hiddenMenu;
    void Start()
    {
        currentChoiceIndex = 0;
        menuActive = false;
    }

    private Dictionary<GameObject, bool> previousStates = new Dictionary<GameObject, bool>();
    void Update()
    {
        if( InputManager.GetInstance().GetESCPressed())
        {
            TogglePause();
        }

        if (!menuActive) return;

        if (InputManager.GetInstance().GetUIUpPressed())
        {
            currentChoiceIndex = Mathf.Max(0, currentChoiceIndex - 1);
        }
        else if (InputManager.GetInstance().GetUIDownPressed())
        {
            currentChoiceIndex = Mathf.Min(choices.Length - 1, currentChoiceIndex + 1);
        }
        UpdateChoiceHighlight();

        if(InputManager.GetInstance().GetSubmitPressed())
        {
            MakeChoise();
        }
        

    }

    private void TogglePause()
    {
        menuActive = !menuActive;
        menuUI.SetActive(menuActive);
        Time.timeScale = menuActive ? 0f : 1f;

        if (menuActive)
        {
            SaveAndHideOtherMenus();
            currentChoiceIndex = 0;
            UpdateChoiceHighlight();
        }
        else
        {
            RestoreMenus();
        }
    }

    private void SaveAndHideOtherMenus()
    {
        previousStates.Clear();

        foreach (var ui in hiddenMenu)
        {
            if (ui != null)
            {
                previousStates[ui] = ui.activeSelf;
                ui.SetActive(false);
            }
        }
    }

    private void RestoreMenus()
    {
        foreach (var kvp in previousStates)
        {
            if (kvp.Key != null)
                kvp.Key.SetActive(kvp.Value);
        }
    }

    private void UpdateChoiceHighlight()
    {
        for (int i = 0; i < choices.Length; i++)
        {
            var text = choices[i].GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.color = (i == currentChoiceIndex) ? Color.red : Color.white;
            }
        }
    }

    private void MakeChoise()
    {
        switch (currentChoiceIndex) 
        {
            case 0:
                TogglePause();
                break;
            
            case 1:
                Time.timeScale = 1f;
                SceneManager.LoadSceneAsync(0);
                break;
        }

    }

}
