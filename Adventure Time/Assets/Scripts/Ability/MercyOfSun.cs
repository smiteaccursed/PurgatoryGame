using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Abilities/Passive/MercyOfSun")]
public class MercyOfSun : PassiveAbility
{

    public override void Apply(GameObject player)
    {
        if (!player.TryGetComponent<MersyOfSunEffect>(out _))
        {
            player.AddComponent<MersyOfSunEffect>();
        }
    }


}
