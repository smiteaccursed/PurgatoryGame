using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodController :MonoBehaviour
{
    Animator animator;
    System.Random rnd;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rnd = new System.Random();
    }

    public void ActivateBlood()
    {
        int number = rnd.Next(1, 5);
        animator.CrossFade($"{number}", 0.1f);
    }
}
