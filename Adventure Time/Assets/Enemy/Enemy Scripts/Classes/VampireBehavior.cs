using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireBehavior : BaseBehavior
{

    public override string ClassName { get; set; } = "вампир";

    public override void OnDamage(EnemyAI enemy)
    {
        enemy.HP += enemy.damage;
    }

}
