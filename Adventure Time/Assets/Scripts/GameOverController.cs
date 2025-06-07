using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public GameObject[] choices;
    public int currentChoiceIndex;
    public bool menuActive;
    public static GameOverController Instance { get; private set; }
    public GameObject menuUI;
    public GameObject[] hiddenMenu;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentChoiceIndex = 0;
        menuActive = false;
    }

    private Dictionary<GameObject, bool> previousStates = new Dictionary<GameObject, bool>();
    void Update()
    {
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

        if (InputManager.GetInstance().GetSubmitPressed())
        {
            MakeChoise();
        }
    }

    public void DeathScreen()
    {
        PlayerData pd = DataManager.Instance.saveData;
        pd.canUse = false;
        SaveSystem.Instance.SaveGame(pd);
        menuActive = true;
        menuUI.SetActive(menuActive);
        Time.timeScale = 0f;

        if (menuActive)
        {
            SaveAndHideOtherMenus();
            currentChoiceIndex = 0;
            UpdateChoiceHighlight();
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
                Time.timeScale = 1f;
                SceneManager.LoadSceneAsync(1);
                break;

            case 1:
                Time.timeScale = 1f;
                SceneManager.LoadSceneAsync(0);
                break;
        }
    }
}
