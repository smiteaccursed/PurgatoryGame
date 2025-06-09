using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject Melee;

    public float startTimeAttack = 1.0f;  
    public float attackAnimDuration = 0.5f;  

    public bool isAttacking { get; private set; }  
    private bool canAttack = true;  

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

    private void Update()
    {
        if (canAttack && InputManager.GetInstance() != null && InputManager.GetInstance().GetLightAttackPressed())
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;
        isAttacking = true;

        animator.SetTrigger("LightAttack");
        meleeRenderer.enabled = true;

        meleeCollider.enabled = true;


        Vector2 originalOffset = meleeCollider.offset;
        meleeCollider.offset += Vector2.right * 0.01f;
        yield return new WaitForSeconds(0.1f);
        meleeCollider.offset = originalOffset;
        //meleeRenderer.enabled = false;
        meleeCollider.enabled = false;
        yield return new WaitForSeconds(attackAnimDuration);

        isAttacking = false;
 

        float remainingDelay = Mathf.Max(0, startTimeAttack - attackAnimDuration);
        yield return new WaitForSeconds(remainingDelay);

        canAttack = true;
    }


    public static PlayerAttack GetInstance()
    {
        return instance;
    }

    public bool GetAttacking()
    {
        return isAttacking;
    }
}
