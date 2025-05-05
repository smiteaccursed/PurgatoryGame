using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public Sprite icon;
}
public abstract class PassiveAbility : Ability
{
    public abstract void Apply(GameObject player);
}

public abstract class ActiveAbility : Ability
{
    public float manaCostPerSecond;
    public AudioClip activationSound;
    public Sprite screenEffect;

    public abstract void StartAbility(GameObject player);
    public abstract void UpdateAbility(GameObject player, float deltaTime);
    public abstract void StopAbility(GameObject player);
}