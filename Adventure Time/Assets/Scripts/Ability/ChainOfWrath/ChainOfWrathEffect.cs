using UnityEngine;

public class ChainOfWrathEffect : MonoBehaviour
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

        stats.ChangeDamage(2);
        stats.changeHP(10);
        stats.ChangeMana(10);
    }

}
