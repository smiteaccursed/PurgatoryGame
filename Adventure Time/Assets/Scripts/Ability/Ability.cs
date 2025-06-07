using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public int id;
    public string abilityName;
    public Sprite icon;
    public string description;
}
public abstract class PassiveAbility : Ability
{
    public abstract void Apply(GameObject player);
}

public abstract class ActiveAbility : Ability
{
    public AudioClip activationSound;

    public abstract void StartAbility(GameObject player);
    public abstract void StopAbility(GameObject player);
}