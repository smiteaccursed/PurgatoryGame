using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RogueBehavior : IEnemyBehavior
{
    public virtual string ClassName { get; set; } = "плут";
    public string GetName()
    {
        return ClassName;
    }

    public virtual void Execute(EnemyAI enemy)
    {

        if (enemy.TargetInSight())
        {
            enemy.lastSeenPosition = enemy.target.position;

            float distanceToTarget = Vector2.Distance(enemy.transform.position, enemy.target.position);
            bool canAttack = (Time.time - enemy.lastAttackTime) > enemy.attackCooldown;


            if (Vector2.Distance(enemy.transform.position, enemy.target.position) <= enemy.attackRange && canAttack)
            {
                enemy.Attack();
            }
            else if(canAttack)
            {
                enemy.moveDirection= (enemy.lastSeenPosition - (Vector2)enemy.transform.position).normalized;
                enemy.MoveTo(enemy.lastSeenPosition, enemy.moveDirection);
            }
            else
            {
                Vector2 fleeDirection = ((Vector2)enemy.transform.position - enemy.lastSeenPosition).normalized;
                Vector2 fleePosition = (Vector2)enemy.transform.position + fleeDirection;

                enemy.moveDirection = fleeDirection;
                enemy.MoveTo(fleePosition, fleeDirection);
            }
        }
        else if (Vector2.Distance(enemy.transform.position, enemy.lastSeenPosition) > 0.5f)
        {
            enemy.MoveTo(enemy.lastSeenPosition, enemy.moveDirection);
        }
        else
        {
            enemy.animator.CrossFade("Idle", 0f);
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
    public virtual Action<EnemyAI, bool> OnNightChange => (enemy, isNight) =>
    {

    };
}
