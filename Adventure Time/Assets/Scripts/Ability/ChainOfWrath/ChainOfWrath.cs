using UnityEngine;
[CreateAssetMenu(menuName = "Abilities/Passive/ChainOfWrath")]
public class ChainOfWrath : PassiveAbility
{
    public override void Apply(GameObject player)
    {
        if (!player.TryGetComponent<ChainOfWrathEffect>(out _))
        {
            Transform passiveAbilitiesParent = player.transform.Find("PassiveAbilities");
            passiveAbilitiesParent.gameObject.AddComponent<ChainOfWrathEffect>();
        }
    }
}
