using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public GameObject Melee;

    public float startTimeAttack = 1.0f;  
    public float attackAnimDuration = 0.5f;  

    public bool isAttacking { get; private set; }  
    private bool canAttack = true;  

    private Animator animator;
    private Animator meleeAnimator;
    private Collider2D meleeCollider;
    private SpriteRenderer meleeRenderer;
    public Image attackIndicator;
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
        meleeAnimator = Melee.GetComponent<Animator>();
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
        meleeAnimator.CrossFade("Swing", 0f);
        meleeCollider.offset = originalOffset;
        //meleeRenderer.enabled = false;
        meleeCollider.enabled = false;
        yield return new WaitForSeconds(attackAnimDuration);
        meleeRenderer.sprite = null;
        meleeRenderer.enabled = false;
        isAttacking = false;
 

        float remainingDelay = Mathf.Max(0, startTimeAttack - attackAnimDuration);
        StartCoroutine(FillAttackIndicator(remainingDelay));

        yield return new WaitForSeconds(remainingDelay);

        canAttack = true;
    }

    private IEnumerator FillAttackIndicator(float fillTime)
    {
        float timer = 0f;

        while (timer < fillTime)
        {
            timer += Time.deltaTime;
            float t = timer / fillTime;

            Color fillColor = Color.Lerp(Color.red, Color.green, t);
            attackIndicator.color = fillColor;

            yield return null;
        }
        attackIndicator.color = Color.white;
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
