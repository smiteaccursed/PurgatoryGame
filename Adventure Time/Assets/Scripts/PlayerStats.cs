using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("Статы игрока")]
    public float hp;
    public float maxHP;
    
    public int lvl =0;
    public float currentEXP=0f;
    public float nextLVLEXP=100f;
    public float baseExp = 10f;

    public float mp;
    public float magicPoint;

    public float damage;
    public float baseDMG;
    public float multDMG=1f;
    public float speed;

    public float attackDelay;
    public float dashDelay;

    [Header("UI статов")]
    public GameObject HealtBar;
    public GameObject ManaBar;
    public GameObject SwordInfo;
    public GameObject SwordDelayInfo;
    public GameObject DashDelayInfo;

    [Header("Связь с механиками")]
    public PlayerMovment movment;
    public Weapon weapon;
    public PlayerAttack attack;

    private static PlayerStats instance;

    public static PlayerStats Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerStats>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("PlayerStats");
                    instance = singletonObject.AddComponent<PlayerStats>();
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        weapon = GetComponentInChildren<Weapon>();
        //attack = GetComponent<PlayerAttack>();
        //movment = GetComponent<PlayerMovment>();
    }

    private void OnEnable()
    {
        HealtBarUpdate();
        SwordInfoUpdate();
    }

    private void Start()
    {
        if (DataManager.Instance.isExist)
        {
            PlayerData pd = DataManager.Instance.saveData;
            baseDMG = pd.baseDMG;
            weapon.damage = baseDMG * multDMG;
            damage = weapon.damage;
            attackDelay = attack.startTimeAttack;
            hp = pd.hp;
            maxHP = pd.maxHP;

            magicPoint = pd.magicPoint;
            mp = pd.mp;

            lvl = pd.lvl;
            currentEXP = pd.currentEXP;
            nextLVLEXP = pd.nextLVLEXP;
            baseExp = pd.baseExp;
        }
        else
        {
            baseDMG = weapon.damage;
            weapon.damage *= multDMG;
            damage = weapon.damage;
        }
        

        HealtBarUpdate();
        SwordInfoUpdate();
        StartCoroutine(RegenerateMana());
        StartCoroutine(RegenerateHP());
        LoadingController.Instance.IsPlayer = true;
    }

    public void changeHP(float mult)
    {
        maxHP += mult;
        hp += mult;
        HealtBarUpdate();
    }

    public void ChangeMana(float mult)
    {
        magicPoint += mult;
        mp += mult;
        ManaBarUpdate();
    }

    public void ChangeDamage(float dmg)
    {
        baseDMG += dmg;
        damage = baseDMG*multDMG;
        weapon.damage = baseDMG * multDMG;
         

        SwordInfoUpdate();
    }

    public void ChangeMultDMG(float chng)
    {
        multDMG = chng;
        ChangeDamage(0f);
    }
    public void HealtBarUpdate()
    {
        float temp = Mathf.Round(hp * 10);
        hp = temp / 10;
        Transform fill = HealtBar.transform.Find("Fill");
        Vector3 scale = fill.localScale;
        scale.x = hp/maxHP;
        fill.localScale = scale;

        TextMeshProUGUI text = HealtBar.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        text.text = $"{hp}/{maxHP}";
    }

    public void ManaBarUpdate()
    {
        float temp = Mathf.Round(mp * 10);
        mp = temp / 10;
        Transform fill = ManaBar.transform.Find("Fill");
        Vector3 scale = fill.localScale;
        scale.x = mp / magicPoint;
        fill.localScale = scale;

        TextMeshProUGUI text = ManaBar.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        text.text = $"{mp}/{magicPoint}";
    }

    public void SwordInfoUpdate()
    {
        TextMeshProUGUI text = SwordInfo.transform.Find("SwordInfo").GetComponent<TextMeshProUGUI>();
        text.text = $"{damage} | {attackDelay}";
    }

    public void GetDamage(float dmg)
    {
        hp -= dmg;
        HealtBarUpdate();
        if (hp<=0)
        {
            hp = 0;
            GameOverController.Instance.DeathScreen();
        }
    }
    public void GetExpReward(int enemyLevel)
    {
        int diff = enemyLevel - lvl;

        if (diff >= 0)
        {
            float multiplier = 1f + Mathf.Pow(Mathf.Max(0, diff - 2), 1.5f) * 0.25f;
            currentEXP+= baseExp * multiplier;
        }
        else
        {
            float penalty = Mathf.Clamp01(1f + diff * 0.2f);
            currentEXP += baseExp * Mathf.Max(penalty, 0.1f);
        }

        if(currentEXP>=nextLVLEXP)
        {
            currentEXP -= nextLVLEXP;
            nextLVLEXP = 100f + Mathf.Log(lvl + 1) * 50f;
            lvl += 1;
            LvlUP();
        }
    }

    public void LvlUP()
    {
        ChangeDamage(1f);
        changeHP(15f);
        ChangeMana(15f);
    }

    private IEnumerator RegenerateMana()
    {
        WaitForSeconds delay = new WaitForSeconds(1f);

        while (true)
        {
            if (mp < magicPoint)
            {
                float regenAmount = magicPoint * 0.01f;
                mp = Mathf.Min(mp + regenAmount, magicPoint);
                ManaBarUpdate();
            }

            yield return delay;
        }
    }

    private IEnumerator RegenerateHP()
    {
        WaitForSeconds delay = new WaitForSeconds(1f);

        while (true)
        {
            if (hp < maxHP)
            {
                float regenAmount = maxHP * 0.01f;
                hp = Mathf.Min(hp + regenAmount, maxHP);
                HealtBarUpdate();
            }

            yield return delay;
        }
    }
}
