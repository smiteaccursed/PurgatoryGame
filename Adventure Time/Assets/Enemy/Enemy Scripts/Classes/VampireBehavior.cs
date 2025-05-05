using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VampireBehavior : BaseBehavior
{

    public override string ClassName { get; set; } = "вампир";

    public override Action<EnemyAI, bool> OnNightChange => (enemy, isNight) =>
    {
        if (isNight)
        {
            enemy.speed = enemy.baseSpeed * 2f;
            enemy.enemyWeapon.damage = enemy.baseDMG * 1.5f;
            enemy.damage = enemy.baseDMG * 1.5f;
        }
        else
        {
            enemy.speed = enemy.baseSpeed / 2f;
            enemy.enemyWeapon.damage = enemy.baseDMG / 1.5f;
            enemy.damage = enemy.baseDMG / 1.5f;
        }
    };
}
