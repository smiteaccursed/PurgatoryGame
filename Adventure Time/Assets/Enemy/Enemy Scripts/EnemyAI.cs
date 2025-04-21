using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Параметры ИИ")]
    public Transform target;
    public float speed = 2f;
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    [Header("Параметры НПС")]
    public float HP;
    public float maxHP;
    public float damage;
    public int level;
    public string enemyName;
    public string enemyClassName;
    public IEnemyBehavior behavior;
    public Transform AttackZone;

    [Header("Внутренние данные для поведения")]
    public Rigidbody2D rb;
    public LayerMask obstaclesLayer;
    private bool isWandering;
    public Vector2 lastSeenPosition;
    public bool targetVisible;
    public Animator animator;
    private bool isAttacking;
    public float lastAttackTime;
    public Vector2 moveDirection;
    public EnemyUI enemyUI;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        lastSeenPosition = transform.position;

        SetBehavior(new VampireBehavior());

        level = Mathf.RoundToInt(rb.position.magnitude / 16);
        HP = maxHP;
        enemyClassName = behavior.GetName();
        enemyUI = GetComponentInChildren<EnemyUI>();
        enemyUI.SetName();
    }

    void Update()
    {
        behavior.Execute(this);

        //if (TargetInSight())
        //{
        //    lastSeenPosition = target.position;
        //    moveDirection = (lastSeenPosition - (Vector2)transform.position).normalized;
        //    if (Vector2.Distance(transform.position, target.position) <= attackRange && Time.time - lastAttackTime > attackCooldown)
        //    {
        //        Attack();

        //    }
        //    else
        //    {
        //        MoveTo(lastSeenPosition, moveDirection);
        //    }
        //}
        //else if (Vector2.Distance(transform.position, lastSeenPosition) > 0.5f)
        //{
        //    MoveTo(lastSeenPosition, moveDirection);
        //}
        //else
        //{
        //    animator.SetFloat("Speed", 0);
        //    Wander();
        //}
    }

    public bool TargetInSight()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance < detectionRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstaclesLayer);
            if (hit.collider == null)
            { 
                targetVisible = true;
                return true;
            }
        }
        targetVisible = false;
        return false;
    }

    public void MoveTo(Vector2 target, Vector2 direction)
    {
        if (isAttacking) return;

        animator.SetFloat("Speed", speed);
        Vector2 prevPos = transform.position;
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        Vector3 v3 = Vector3.left * direction.x + Vector3.down * direction.y;
        AttackZone.rotation = Quaternion.LookRotation(Vector3.forward, v3);

        animator.SetFloat("DirectionX", direction.x);
        animator.SetFloat("DirectionY", direction.y);
        //animator.SetFloat("Speed", actualSpeed);
        //AnimationUpdate(direction);
    }

    public void Wander()
    {
        if (isWandering || targetVisible) return;

        isWandering = true;
        InvokeRepeating("SetRandomDestination", 0f, Random.Range(2f, 5f));
    }
    
    void SetRandomDestination()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized * 3f;
        lastSeenPosition = (Vector2)transform.position + randomDirection;
        moveDirection = (lastSeenPosition - (Vector2)transform.position);

        animator.SetFloat("DirectionX", moveDirection.x);
        animator.SetFloat("DirectionY", moveDirection.y);
    }
    public void Attack()
    {
        isAttacking = true;
        //animator.SetTrigger("Attack");
        animator.CrossFade("Attack", 0.1f);
        lastAttackTime = Time.time;

        StartCoroutine(ResetAttackState());
    }

    IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        if (!isAttacking)
        {
            rb.constraints = RigidbodyConstraints2D.None; 
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
        }
    }


    public void SetBehavior(IEnemyBehavior newBehavior)
    {
        behavior = newBehavior;
    }

    public void TakeDamage(float damage)
    {
        animator.CrossFade("Hurt", 0.1f);
        HP -= damage;
        if (HP <= 0)
        {
            animator.CrossFade("Death", 0.1f);
            Destroy(gameObject);
        }
    }
}
 
