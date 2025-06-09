using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEaterEffect : MonoBehaviour
{
    private PlayerStats stats;

    private void Start()
    {
        stats = FindObjectOfType<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats not found on object!");
            return;
        }
        stats.weapon.hitting += ManaRegen;
        
    }

    private void OnDestroy()
    {
        stats.weapon.hitting -= ManaRegen;
    }

    private void ManaRegen()
    {
        stats.AddMana(10f);
    }
}
