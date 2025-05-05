using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerAbilities : MonoBehaviour
{
    public List<PassiveAbility> passiveAbilities = new List<PassiveAbility>();
    public ActiveAbility activeAbility;

    private bool isUsingActive;
    private GameObject player;
    public GameObject abilityOverlay;

    private void Start()
    {
        player = gameObject;
        abilityOverlay.SetActive(false);

        foreach (var passive in passiveAbilities)
        {
            Debug.Log("Применяем пассивку: " + passive.name);
            passive.Apply(player);
        }
    }

    public void AddPassive(PassiveAbility ability)
    {
        passiveAbilities.Add(ability);
        ability.Apply(player);
    }

    public void SetActiveAbility(ActiveAbility ability)
    {
        activeAbility = ability;
    }

    private void Update()
    {
        if (activeAbility == null)
            return;

    }
}

