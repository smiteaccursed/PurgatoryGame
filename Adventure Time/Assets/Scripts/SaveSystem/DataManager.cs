using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public List<string> activatedStatue =new List<string>();
    public List<AbilityData> abilityOnFloor = new List<AbilityData>();
    public List<int> currentActive = new List<int>();
    public List<int> currentPassive = new List<int>();
    public PlayerData saveData;
    public GameObject player;
    public bool isExist = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;

        saveData = SaveSystem.Instance.LoadGame();
        if(saveData!=null && saveData.canUse)
        {
            isExist = true;
            //player.transform.position = saveData.playerPosition;
            activatedStatue = saveData.entities.statueData;
            abilityOnFloor = saveData.entities.abilityDatas;
            currentActive = saveData.currentActive;
            currentPassive = saveData.currentPassive;
        }
    }
}
