using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Active/Kamui")]
public class Kamui : ActiveAbility
{

    public float manaCostPerSecond = 8f;

    public override void StartAbility(GameObject player)
    {
        if (!player.TryGetComponent<KamuiEffect>(out var effect))
        {
            effect = player.AddComponent<KamuiEffect>();
        }

        effect.ToggleEffect(this); // передаем себя, чтобы получить параметры
    }

    public override void StopAbility(GameObject player)
    {
        if (player.TryGetComponent<KamuiEffect>(out var effect))
        {
            effect.ForceStop();
        }
    }
}
