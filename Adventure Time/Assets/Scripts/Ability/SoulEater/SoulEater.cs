using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Abilities/Passive/SoulEater")]
public class SoulEater : PassiveAbility
{
    public override void Apply(GameObject player)
    {
        if (!player.TryGetComponent<SoulEaterEffect>(out _))
        {
            Transform passiveAbilitiesParent = player.transform.Find("PassiveAbilities");
            passiveAbilitiesParent.gameObject.AddComponent<SoulEaterEffect>();
        }
    }
}
