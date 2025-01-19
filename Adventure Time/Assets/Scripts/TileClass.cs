using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass : MonoBehaviour
{
    public float health=30;
    public int bit = -1;
 
    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health<=0)
        {
            OnDestroy();
        }
    }

    private void OnDestroy()
    {
        Vector2Int blockPosition = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        // ”дал€ем блок
        Destroy(gameObject);

        // ѕолучаем чанк, в котором находитс€ блок (если используетс€ система чанков)
        WorldManager.Chunk currentChunk = WorldManager.GetInstance().FindChunkAtPosition(new Vector2Int(0,0));

        // ќбновл€ем спрайты соседей
        currentChunk.UpdateTileAndNeighbors(blockPosition);
        Destroy(gameObject);
    }
}
