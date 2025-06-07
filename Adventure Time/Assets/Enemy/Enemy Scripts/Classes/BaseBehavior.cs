using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseBehavior : IEnemyBehavior
{
    public virtual string ClassName { get; set; } = "нормис";
    public string GetName()
    {
        return ClassName;
    }

    public virtual void Execute(EnemyAI enemy)
    {

        if (enemy.TargetInSight())
        {
            enemy.lastSeenPosition = enemy.target.position;
            enemy.moveDirection = (enemy.lastSeenPosition - (Vector2)enemy.transform.position).normalized;
            if (Vector2.Distance(enemy.transform.position, enemy.target.position) <= enemy.attackRange && Time.time - enemy.lastAttackTime > enemy.attackCooldown)
            {
                enemy.Attack();
            }
            else
            {
                enemy.MoveTo(enemy.lastSeenPosition, enemy.moveDirection);
            }
        }
        else if (Vector2.Distance(enemy.transform.position, enemy.lastSeenPosition) > 0.5f)
        {
            enemy.MoveTo(enemy.lastSeenPosition, enemy.moveDirection);
        }
        else
        {
            enemy.animator.CrossFade("Idle",0f);
            enemy.animator.SetFloat("Speed", 0);
            enemy.Wander();
        }
    }

    public virtual void OnDamage(EnemyAI enemy)
    {

    }
    public virtual void OnDeath(EnemyAI enemy)
    {

    }
    public virtual void OnHurt(EnemyAI enemy)
    {

    }

    public virtual void OnHit(EnemyAI enemy)
    {

    }
    public virtual Action<EnemyAI, bool> OnNightChange => (enemy, isNight) =>
    {

    };
}
