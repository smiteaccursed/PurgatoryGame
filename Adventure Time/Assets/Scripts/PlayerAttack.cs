using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject Melee; // attack location

    private bool isLAttacking;
    public bool isAttaking;

    public float attackTime;
    public float startTimeAttack;

   //public LayerMask enemies;

    private Animator animator;
    private Collider2D meleeCollider;
    private SpriteRenderer meleeRenderer;

    private static PlayerAttack instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;

        animator = GetComponent<Animator>();
        meleeCollider = Melee.GetComponent<Collider2D>();
        meleeRenderer = Melee.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //if (isLAttacking)
        //{
        //    CheckMeleeTimer();
        //}

        //if (InputManager.GetInstance()!=null && InputManager.GetInstance().GetLightAttackPressed())
        //{
        //    isAttaking = true;
        //    OnLightAttack();
        //}

        if(attackTime <=0)
        {
            if (InputManager.GetInstance() != null && InputManager.GetInstance().GetLightAttackPressed())
            {
                OnLightAttack();
                attackTime = startTimeAttack;
            }
            else
            {
                DisableAttackFlag();
            }
        }
        else
        {
            attackTime -= Time.deltaTime;
            DisableLAFlags();
        }
    }

    public static PlayerAttack GetInstance()
    {
        return instance;
    }

    void OnLightAttack()
    {
        if (!isLAttacking)
        {
            meleeCollider.enabled = true;
            //meleeRenderer.enabled = true;
            isLAttacking = true;
            isAttaking = true;
            animator.SetBool("LightAttack", true);
        }
    }

    void DisableLAFlags()
    {
        isLAttacking = false;
 
        animator.SetBool("LightAttack", false);
    }
    
    void DisableAttackFlag()
    {
        isAttaking = false;
        meleeCollider.enabled = false;
        meleeRenderer.enabled = false;
    }

    public bool GetAttacking()
    {
        return isAttaking;
    }
}
