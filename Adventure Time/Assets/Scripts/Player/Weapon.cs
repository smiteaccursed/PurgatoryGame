using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Weapon : MonoBehaviour
{
    public float damage = 30;
    public event Action hitting;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TileClass block = collision.GetComponent<TileClass>();
        if (block != null && block.isbuf)
        {
            block.TakeDamage(damage);
            hitting?.Invoke();
        }
        EnemyAI enemyAI = collision.GetComponent<EnemyAI>();
        if(enemyAI!= null)
            enemyAI.TakeDamage(damage);
        BossAI bossAi = collision.GetComponent<BossAI>();
        if(bossAi!=null)
        {
            bossAi.GetDamage(damage);
        }

    }
}
