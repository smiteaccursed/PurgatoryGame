using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerAbilities : MonoBehaviour
{
    public List<PassiveAbility> passiveAbilities = new List<PassiveAbility>();
    public ActiveAbility activeAbility;
    public AudioSource source;
    private GameObject player;
    public GameObject icon;
    private void Start()
    {
        player = gameObject;
        foreach (var passive in passiveAbilities)
        {
            Debug.Log("œËÏÂÌˇÂÏ Ô‡ÒÒË‚ÍÛ: " + passive.name);
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
    }

    public void SetActiveAbility(ActiveAbility ability)
    {
        Image img = icon.GetComponent<Image>();
        img.sprite = ability.icon;
        activeAbility = ability;
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
            //
            //Debug.Log("¿¡»À ¿¿¿¿¿¿¿");
        }

    }
}

