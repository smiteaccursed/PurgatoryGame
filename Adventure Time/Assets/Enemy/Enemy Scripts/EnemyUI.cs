using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI: MonoBehaviour 
{
    public Text enemyUI;
    private EnemyAI enemyAI;

    void Start()
    {
        enemyUI = GetComponentInChildren<Text>();
        enemyAI = GetComponentInParent<EnemyAI>();
        //SetName();
    }

    public void SetName()
    {
        string name = $"Lvl. {enemyAI.level}  {enemyAI.enemyName} - {enemyAI.enemyClassName} \n {enemyAI.HP}/{enemyAI.maxHP} ";
        enemyUI.text = name;
    }
}
