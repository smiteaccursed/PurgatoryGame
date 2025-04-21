using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Статы игрока")]
    public double hp;
    public double maxHP;

    public double damage;
    public double speed;

    public double attackDelay;
    public double dashDelay;

    public double mp;
    public double magicPoint;


    [Header("Связь с механиками")]
    public PlayerMovment movment;
    public Weapon weapon;
    public PlayerAttack attack;


    private void Awake()
    {
        weapon = GetComponentInChildren<Weapon>();
        //attack = GetComponent<PlayerAttack>();
        //movment = GetComponent<PlayerMovment>();
    }

    private void Start()
    {
        damage = weapon.damage;
        speed = movment.moveSpeed;
        attackDelay = attack.startTimeAttack;
    }

    public void changeHP(double mult)
    {
        maxHP *= mult;
        hp *= mult;
    } 
    
    public void changeDamage(float dmg)
    {
        damage += dmg;
        weapon.damage += dmg;
    }
}
