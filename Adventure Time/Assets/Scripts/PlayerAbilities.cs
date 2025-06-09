using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerAbilities : MonoBehaviour
{
    public List<PassiveAbility> passiveAbilities = new List<PassiveAbility>();
    public ActiveAbility activeAbility;
    public AudioSource source;
    private GameObject player;
    public GameObject icon;
    public TextMeshProUGUI abilityName;
    public TextMeshProUGUI abilityDescriprion;

    private static PlayerAbilities Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = gameObject;
        foreach (var passive in passiveAbilities)
        {
            Debug.Log("Применяем пассивку: " + passive.name);
            passive.Apply(player);
        }

        if(DataManager.Instance.isExist)
        {
            PlayerData pd = DataManager.Instance.saveData;
            if(pd.activeAbilityID>0)
            {
                ScriptableObject aa =AbilitiesManager.Instance.FindAbilityById(pd.activeAbilityID);
                SetActiveAbility((ActiveAbility)aa);
            }
            if(pd.passiveAbilityIDs.Count>0)
            {
                foreach (int id in pd.passiveAbilityIDs)
                {
                    ScriptableObject ability = AbilitiesManager.Instance.FindAbilityById(id);
                    if (ability is PassiveAbility passive)
                    {
                        AddPassive((PassiveAbility)ability);
                    }
                }
            }
        }
    }

    public void AddPassive(PassiveAbility ability)
    {
        passiveAbilities.Add(ability);
        ability.Apply(player);
        StartCoroutine(ShowAbilityInfo(ability.abilityName, ability.description));
    }

    public void SetActiveAbility(ActiveAbility ability)
    {
        Image img = icon.GetComponent<Image>();
        img.sprite = ability.icon;
        activeAbility = ability;
        StartCoroutine(ShowAbilityInfo(ability.abilityName, ability.description));
    }

    private void Update()
    {
        if (activeAbility == null)
            return;
        
        if(InputManager.GetInstance().GetActivateAbilityPressed())
        {
            if(activeAbility.activationSound!=null)
            {
                source.PlayOneShot(activeAbility.activationSound);
            }
            activeAbility.StartAbility(gameObject); ;
        }

    }

    IEnumerator ShowAbilityInfo(string name, string desc)
    {
        abilityName.text = name;
        abilityDescriprion.text = desc;

        yield return new WaitForSeconds(3f);

        abilityName.text = "";
        abilityDescriprion.text = "";
    }
}

