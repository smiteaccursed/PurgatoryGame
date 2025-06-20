using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    [SerializeField] public List<GameObject> plants;
    public static PlantController Instance { get; private set; }

    private void Awake()
    {
        if(Instance!=null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void SpawnRNDPlant(Vector3 pos, Transform tr)
    {
        GameObject plnt = GameObject.Instantiate(plants[Random.Range(0, plants.Count)], pos, Quaternion.identity);
        plnt.transform.SetParent(tr);
        return;
    }
}
