using System.Collections;
using UnityEngine;

public class THEWORLDEffect : MonoBehaviour
{
    private PlayerStats stats;
    private Coroutine manaDrainCoroutine;
    private bool isActive = false;

    public void ToggleEffect(THEWORLD ability)
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();

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
            Debug.Log("ZA WARUDO");
            TimeManger.GetInstance().TriggerTimeStop();
            manaDrainCoroutine = StartCoroutine(ManaDrain(ability));
        }
    }

    public void ForceStop()
    {
        if (!isActive) return;

        isActive = false;
        if (manaDrainCoroutine != null)
            StopCoroutine(manaDrainCoroutine);
        Debug.Log("Чет не за варудо");
        TimeManger.GetInstance().TriggerTimeResume();
    }

    private IEnumerator ManaDrain(THEWORLD ability)
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
