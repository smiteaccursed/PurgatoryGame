using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyWeapon : MonoBehaviour
{
    public float damage = 30;
    public event Action HittingPlayer;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerStats playerStats = collision.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.GetDamage(damage);
            HittingPlayer?.Invoke();
        }

    }
}
