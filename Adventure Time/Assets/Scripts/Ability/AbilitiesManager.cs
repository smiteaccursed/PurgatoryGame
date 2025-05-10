using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesManager : MonoBehaviour
{
    public List<PassiveAbility> passiveAbilities = new List<PassiveAbility>();
    public List<ActiveAbility> activeAbilities = new List<ActiveAbility>();

    public GameObject abilityBase;
    private static AbilitiesManager instance;


    private void Start()
    {
        if(DataManager.Instance.isExist)
        {
            foreach(var ability in DataManager.Instance.saveData.entities.abilityDatas)
            {
                RecreateAbility(ability.pos, ability.name, ability.ID);
            }
        }
    }

    public static AbilitiesManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AbilitiesManager>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("PlayerStats");
                    instance = singletonObject.AddComponent<AbilitiesManager>();
                }
            }
            return instance;
        }
    }
    public void RecreateAbility(Vector3 pos, string aName, int ID)
    {
        GameObject go = GameObject.Instantiate(abilityBase, pos, Quaternion.identity);
        go.name = aName;
        ScriptableObject ability = FindAbilityById(ID);
        if(ability is ActiveAbility)
        {
            go.GetComponent<AbilityTrigger>().activeAbility = (ActiveAbility)ability;
        }
        else if(ability is PassiveAbility)
        {
            go.GetComponent<AbilityTrigger>().passiveAbility = (PassiveAbility)ability;
        }

    }
    public void CreateArt(Vector3 pos)
    {
        GameObject go = GameObject.Instantiate(abilityBase, pos, Quaternion.identity);
        bool choosePassive = Random.value < 0.5f;
        go.name = $"A{pos.x}.{pos.y}";
        if ((choosePassive && passiveAbilities.Count > 0) || activeAbilities.Count == 0)
        {
            PassiveAbility chosen = passiveAbilities[Random.Range(0, passiveAbilities.Count)];
            go.GetComponent<AbilityTrigger>().passiveAbility = chosen;
            AbilityData ad = new AbilityData(go.name, pos, chosen.id);
            DataManager.Instance.abilityOnFloor.Add(ad);
        }
        else if (activeAbilities.Count > 0)
        {
            ActiveAbility chosen = activeAbilities[Random.Range(0, activeAbilities.Count)];
            go.GetComponent<AbilityTrigger>().activeAbility = chosen;
            AbilityData ad = new AbilityData(go.name, pos, chosen.id);
            DataManager.Instance.abilityOnFloor.Add(ad);
        }
        else
        {
            Debug.LogWarning("Ќет доступных способностей дл€ выбора!");
        }

         
    }

    public ScriptableObject FindAbilityById(int id)
    {
        if (id%2!=0)
            return activeAbilities.Find(a => a.id == id);
        else if (id % 2 == 0)
            return passiveAbilities.Find(p => p.id == id);
        return null;
    }


}
