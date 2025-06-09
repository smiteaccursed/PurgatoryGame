using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyUI: MonoBehaviour 
{
    public TextMeshProUGUI enemyUI;
    private EnemyAI enemyAI;
    private void Awake()
    {
        enemyUI = GetComponentInChildren<TextMeshProUGUI>();
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
