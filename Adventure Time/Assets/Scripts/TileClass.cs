using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass : MonoBehaviour
{
    public float health=100;
    public int bit = -1;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health<=0)
        {
            Destroy(gameObject);
        }
    }
}
