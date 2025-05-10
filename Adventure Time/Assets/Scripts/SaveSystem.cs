using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public class SaveSystem : MonoBehaviour
{
    public GameObject player;
    public static SaveSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SaveGame(PlayerData data)
    {
        string savePath = GetSavePath();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game Saved to: " + savePath);
    }

    public PlayerData LoadGame()
    {
        string savePath = GetSavePath();
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Save file not found");
            return null;
        }

        string json = File.ReadAllText(savePath);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        return data;
    }
    public static bool SaveExists()
    {
        return File.Exists(GetSavePath());
    }

    private static string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "playerSave.json");
    }

    public PlayerData GetAllData()
    {
        
        PlayerData playerData = new PlayerData();
        playerData.seed = WorldManager.GetInstance().seed;
        playerData.canUse = true;
        PlayerStats ps = player.GetComponent<PlayerStats>();

        playerData.playerPosition = player.transform.position;

        playerData.hp = ps.hp;
        playerData.maxHP = ps.maxHP;

        playerData.magicPoint = ps.magicPoint;
        playerData.mp = ps.mp;

        playerData.lvl = ps.lvl;
        playerData.currentEXP = ps.currentEXP;
        playerData.nextLVLEXP = ps.nextLVLEXP;
        playerData.baseExp = ps.baseExp;

        playerData.baseDMG = ps.baseDMG;
        playerData.speed= ps.speed;

        playerData.attackDelay = ps.attackDelay;
        playerData.dashDelay = ps.dashDelay;

        PlayerAbilities pa = player.GetComponent<PlayerAbilities>();
        if (pa.activeAbility != null)
            playerData.activeAbilityID = pa.activeAbility.id;

        if (pa.passiveAbilities != null && pa.passiveAbilities.Count != 0)
            playerData.passiveAbilityIDs = pa.passiveAbilities.Select(item => item.id).ToList();

        if (playerData.entities == null)
            playerData.entities = new EntitiesData();

        playerData.entities.statueData = DataManager.Instance.activatedStatue;
        playerData.entities.abilityDatas = DataManager.Instance.abilityOnFloor;

        return playerData;
    }
}

[System.Serializable]
public class PlayerData
{
    public bool canUse;
    public int seed;
    public Vector3 playerPosition;

    public float hp;
    public float maxHP;

    public int lvl;
    public float currentEXP;
    public float nextLVLEXP;
    public float baseExp;

    public float mp;
    public float magicPoint;

    public float baseDMG;
    public float speed;

    public float attackDelay;
    public float dashDelay;

    // PlayerAbilities
    public int activeAbilityID;
    public List<int> passiveAbilityIDs;

    public EntitiesData entities;
}

[System.Serializable]
public class EntitiesData
{
    public List<string> statueData;
    public List<AbilityData> abilityDatas;
}

[System.Serializable]
public class AbilityData
{
    public string name;
    public Vector3 pos;
    public int ID;

    public AbilityData(string name, Vector3 pos, int ID)
    {
        this.name = name;
        this.pos = pos;
        this.ID = ID;
    }
}