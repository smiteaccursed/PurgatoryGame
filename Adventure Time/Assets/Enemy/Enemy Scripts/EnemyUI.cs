using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI: MonoBehaviour 
{
    public Text enemyUI;
    private EnemyAI enemyAI;
    private void Awake()
    {
        enemyUI = GetComponentInChildren<Text>();
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    void Start()
    {
        //SetName();
    }

    public void SetName()
    {
        string name = $"Lvl. {enemyAI.level}  {enemyAI.enemyName} - {enemyAI.enemyClassName} \n {enemyAI.HP}/{enemyAI.maxHP} ";
        enemyUI.text = name;
    }
}
