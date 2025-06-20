using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MersyOfSunEffect : MonoBehaviour
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

        TimeManger.OnNightStateChanged += UpdateDamageModifier;
        UpdateDamageModifier(TimeManger.GetInstance().isLights);
    }

    private void OnDestroy()
    {
        TimeManger.OnNightStateChanged -= UpdateDamageModifier;
    }

    private void UpdateDamageModifier(bool isNight)
    {
        if (stats == null) return;

        if (isNight)
            stats.ChangeMultDMG(0.1f);
        else
            stats.ChangeMultDMG(1.9f);
    }
}
