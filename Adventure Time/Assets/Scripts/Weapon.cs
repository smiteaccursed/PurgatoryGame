using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 30;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TileClass block = collision.GetComponent<TileClass>();
        if (block != null && block.isbuf)
        {
            block.TakeDamage(damage);
        }
        EnemyAI enemyAI = collision.GetComponent<EnemyAI>();
        if(enemyAI!= null)
        enemyAI.TakeDamage(damage);

    }
}
