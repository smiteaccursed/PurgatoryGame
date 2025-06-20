using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesManager : MonoBehaviour
{
    public List<PassiveAbility> passiveAbilities = new List<PassiveAbility>();
    public List<ActiveAbility> activeAbilities = new List<ActiveAbility>();
    public List<ActiveAbility> currentActive = new List<ActiveAbility>();
    public List<PassiveAbility> currentPassive = new List<PassiveAbility>();
    public GameObject abilityBase;
    private static AbilitiesManager instance;


    private void Start()
    {
        if(DataManager.Instance.isExist && DataManager.Instance.saveData.canUse)
        {
            foreach(var ability in DataManager.Instance.saveData.entities.abilityDatas)
            {
                RecreateAbility(ability.pos, ability.name, ability.ID);
            }
            foreach(var pas in DataManager.Instance.saveData.currentPassive)
            {
                currentPassive.Add((PassiveAbility)FindAbilityById(pas));
            }
            foreach (var ac in DataManager.Instance.saveData.currentActive)
            {
                currentActive.Add((ActiveAbility)FindAbilityById(ac));
            }
        }
        else
        {
            currentActive = new List<ActiveAbility>( activeAbilities);
            currentPassive = new List<PassiveAbility>(passiveAbilities);

            foreach(var pas in currentPassive)
            {
                DataManager.Instance.currentPassive.Add(pas.id);
            }
            foreach (var ac in currentActive)
            {
                DataManager.Instance.currentActive.Add(ac.id);
            }
        }
        LoadingController.Instance.IsAbility = true;
    }

    public static AbilitiesManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<AbilitiesManager>();

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
        int id;
        if ((choosePassive && currentPassive.Count > 0) || activeAbilities.Count == 0)
        {
            PassiveAbility chosen = currentPassive[Random.Range(0, currentPassive.Count)];
            go.GetComponent<AbilityTrigger>().passiveAbility = chosen;
            AbilityData ad = new AbilityData(go.name, pos, chosen.id);
            currentPassive.Remove(chosen);
            DataManager.Instance.currentPassive.Remove(chosen.id);
            DataManager.Instance.abilityOnFloor.Add(ad);
        }
        else if (currentActive.Count > 0)
        {
            ActiveAbility chosen = currentActive[Random.Range(0, currentActive.Count)];
            currentActive.Remove(chosen);
            go.GetComponent<AbilityTrigger>().activeAbility = chosen;
            AbilityData ad = new AbilityData(go.name, pos, chosen.id);
            DataManager.Instance.currentActive.Remove(chosen.id);
            DataManager.Instance.abilityOnFloor.Add(ad);
        }
        else
        {
            PassiveAbility pa = (PassiveAbility)FindAbilityById(8);
            go.GetComponent<AbilityTrigger>().passiveAbility = pa;
            AbilityData ad = new AbilityData(go.name, pos, pa.id);
            DataManager.Instance.abilityOnFloor.Add(ad);
            Debug.LogWarning("Ќет доступных способностей дл€ выбора!");
        }
    }

    public ScriptableObject FindAbilityById(int id)
    {
        ScriptableObject temp = passiveAbilities.Find(a => a.id == id);
        if (passiveAbilities.Find(p => p.id == id) != null)
        {
            return passiveAbilities.Find(p => p.id == id);
        }
        else if (activeAbilities.Find(a => a.id == id) != null)
        {
            return activeAbilities.Find(a => a.id == id);
        }
        else
        {
            return null;
        }
    }
}
