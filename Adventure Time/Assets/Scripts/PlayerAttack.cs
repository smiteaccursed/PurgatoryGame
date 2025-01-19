using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject Melee;
    private bool isLAttacking = false;
    private bool isAttaking = false;
    private float atkDuration = 0.1f;
    private float atkTimer = 0f;
    private static float atkAnimConst = 0.5f;
    private float atkAnimLTimer = atkAnimConst;
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
        if (isLAttacking)
        {
            CheckMeleeTimer();
        }

        if (InputManager.GetInstance()!=null && InputManager.GetInstance().GetLightAttackPressed())
        {
            isAttaking = true;
            OnLightAttack();
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
            meleeRenderer.enabled = true;
            isLAttacking = true;
            isAttaking = true;
            atkAnimLTimer = atkAnimConst;
            animator.SetBool("LightAttack", true);
        }
    }

    void CheckMeleeTimer()
    {
        atkTimer += Time.deltaTime;
        if (atkTimer >= atkDuration)
        {
            atkTimer = 0;
            isLAttacking = false;
            meleeCollider.enabled = false;
            meleeRenderer.enabled = false;
            isAttaking = false;
            animator.SetBool("LightAttack", false);
        }
    }

    public bool GetAttacking()
    {
        return isAttaking;
    }
}
