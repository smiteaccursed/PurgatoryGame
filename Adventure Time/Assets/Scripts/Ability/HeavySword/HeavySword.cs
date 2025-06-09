using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Abilities/Passive/HeavySword")]
public class HeavySword : PassiveAbility
{
    public override void Apply(GameObject player)
    {
        if (!player.TryGetComponent<HeavySwordEffect>(out _))
        {
            Transform passiveAbilitiesParent = player.transform.Find("PassiveAbilities");
            passiveAbilitiesParent.gameObject.AddComponent<HeavySwordEffect>();
        }
    }
}
