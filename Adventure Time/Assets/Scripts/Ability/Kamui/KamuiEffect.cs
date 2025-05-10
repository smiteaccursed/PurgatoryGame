using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamuiEffect : MonoBehaviour
{
    private PlayerStats stats;
    private Coroutine manaDrainCoroutine;
    private bool isActive = false;
    private CapsuleCollider2D playerCC;
    private SpriteRenderer playerSR;
    public void ToggleEffect(Kamui ability)
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();
        if (playerCC == null)
            playerCC = GetComponent<CapsuleCollider2D>();
        if (playerSR == null)
            playerSR = GetComponent<SpriteRenderer>();

        if (!stats)
        {
            Debug.LogError("PlayerStats not found!");
            return;
        }

        if (isActive)
        {
            ForceStop();
        }
        else if (stats.mp >= ability.manaCostPerSecond)
        {
            isActive = true;
            Color newColor = playerSR.color;
            newColor.a = 0.5f;
            playerSR.color = newColor;
            playerCC.enabled=false;
            manaDrainCoroutine = StartCoroutine(ManaDrain(ability));
        }
    }

    public void ForceStop()
    {
        if (!isActive) return;

        isActive = false;
        if (manaDrainCoroutine != null)
            StopCoroutine(manaDrainCoroutine);
        playerCC.enabled = true;
        Color newColor = playerSR.color;
        newColor.a = 1f;
        playerSR.color = newColor;
    }

    private IEnumerator ManaDrain(Kamui ability)
    {
        while (isActive && stats.mp >= ability.manaCostPerSecond)
        {
            yield return new WaitForSeconds(1f);
            stats.mp -= ability.manaCostPerSecond;
            stats.ManaBarUpdate();
        }

        ForceStop();
    }
}
