using UnityEngine;

public class BerserkBehavior : BaseBehavior
{
    public override string ClassName { get; set; } = "берсерк";

    public override void OnDamage(EnemyAI enemy)
    {
        float dmg = enemy.baseDMG * (enemy.HP / enemy.maxHP);
        enemy.ChangeDamage(dmg);
    }
}
