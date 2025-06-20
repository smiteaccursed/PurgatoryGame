using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavySwordEffect : MonoBehaviour
{
    private PlayerStats stats;
    private void Start()
    {
        stats = FindFirstObjectByType<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats not found on object!");
            return;
        }

        stats.ChangeAbilityDamage(20);
        stats.ChangeAttackDelay(2f);
        
    }
}
