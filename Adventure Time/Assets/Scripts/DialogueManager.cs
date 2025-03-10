using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;
    private Story currentStory;

    [SerializeField] private GameObject player;
    private PlayerMovment playerScript;

    public int currentChoiceIndex = 0;

    private bool dialogueIsPlaying;
    public bool hasChoise = false;

    private static DialogueManager instance;

    public bool getDialogueIsPlaying()
    {
        return dialogueIsPlaying;
    }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");

        }
        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach( GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if(!dialogueIsPlaying)
        {
            return;
        }

        if (InputManager.GetInstance().GetSubmitPressed())
        {
            if(hasChoise)
            MakeChoice(currentChoiceIndex);
            
            ContinueStory();
        }


        if (choices.Length>0)
        {
            if(InputManager.GetInstance().GetUIUpPressed())
            {
                currentChoiceIndex = Mathf.Max(0, currentChoiceIndex - 1);
            }

            if(InputManager.GetInstance().GetUIDownPressed())
            {
                currentChoiceIndex = Mathf.Min(choices.Length - 1, currentChoiceIndex + 1);
            }
            UpdateChoiceHighlight();

        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            DisplayChoices();
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        if (currentChoices.Count > 0)
        {
            hasChoise = true;
        }
        else
        {
            hasChoise = false;
        }
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("Выборов больше, чем юи может вместить. Номер выбора:" + currentChoices.Count);
        }

        int index = 0;
        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        for (int i = index; i<choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        currentChoiceIndex = 0;
        UpdateChoiceHighlight();
    }

    private void UpdateChoiceHighlight()
    {
        for (int i = 0; i < choices.Length; i++)
        {
            Button button = choices[i].GetComponent<UnityEngine.UI.Button>();

            if (i == currentChoiceIndex)
            {
                ColorBlock colors = button.colors;
                colors.normalColor = Color.black; // Цвет при выделении
                button.colors = colors;
            }
            else
            {
                ColorBlock colors = button.colors;
                colors.normalColor = Color.grey; // Цвет по умолчанию
                button.colors = colors;
            }
        }
    }
    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        hasChoise = false;
        //ContinueStory();
    }
}

