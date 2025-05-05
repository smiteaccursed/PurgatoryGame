using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyBase;
    List<IEnemyBehavior> behaviors;
    private static EnemyManager instance;

    public static EnemyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EnemyManager>();

                if (instance == null)
                {
                    Debug.LogError("No EnemyManager found in the scene.");
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        behaviors = new List<IEnemyBehavior>
        {
            new BaseBehavior(),
            new VampireBehavior(),
            new RogueBehavior()
        };
    }

    public void GenerateEnemy(int count, Vector2Int pos, GameObject chunk, Vector2Int nm)
    {
        for(int i=0; i<count; i++)
        {
            Vector3Int spawnpos = new Vector3Int(pos.x, pos.y, 0);
            GameObject go = GameObject.Instantiate(enemyBase, spawnpos, Quaternion.identity);
            go.GetComponentInChildren<EnemyAI>().SetBehavior(behaviors[Random.Range(0, behaviors.Count)]);
            go.transform.SetParent(chunk.transform.Find("Enemies"));
            go.name = $"E{nm.x}.{nm.y}.{i}";
        }
    }

}
