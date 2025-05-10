using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Active/THEWORLD")]
public class THEWORLD : ActiveAbility
{
    public float manaCostPerSecond = 10f;

    public override void StartAbility(GameObject player)
    {
        if (!player.TryGetComponent<THEWORLDEffect>(out var effect))
        {
            effect = player.AddComponent<THEWORLDEffect>();
        }

        effect.ToggleEffect(this); // передаем себя, чтобы получить параметры
    }

    public override void StopAbility(GameObject player)
    {
        if (player.TryGetComponent<THEWORLDEffect>(out var effect))
        {
            effect.ForceStop();
        }
    }
}
