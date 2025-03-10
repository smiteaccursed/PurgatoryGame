using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    private void Awake()
    {
        Strength = 5;
        Speed = 3;
        Initiative = 5;
        Health = 100;
        BaseCost = 10;
    }
}
