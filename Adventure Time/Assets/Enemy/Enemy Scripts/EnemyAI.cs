using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Параметры ИИ")]
    public Transform target;
    public float speed = 2f;
    public float baseSpeed=2f;
    public float detectionRange = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    [Header("Параметры НПС")]
    public float HP;
    public float maxHP;
    public float damage;
    public float baseDMG;
    public int level;
    public string enemyName;
    public string enemyClassName;
    public IEnemyBehavior behavior;
    public Transform AttackZone;
    public GameObject Weapon;
    public EnemyWeapon enemyWeapon;

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
    public float attackAnimDuration = 0.5f;
    private Collider2D meleeCollider;
    private SpriteRenderer meleeRenderer;
    public bool isFrozen;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyWeapon = Weapon.GetComponent<EnemyWeapon>();
        lastSeenPosition = transform.position;

        if(behavior == null)
            SetBehavior(new BaseBehavior());

        level = Mathf.RoundToInt(rb.position.magnitude / 16);
        maxHP = maxHP * Mathf.Pow(1.035f, level);
        enemyWeapon.HittingPlayer += HitPlayer;
        enemyWeapon.damage *= Mathf.Pow(1.02f, level);
        damage = enemyWeapon.damage;
        baseDMG = damage;
        HP = maxHP;
        enemyClassName = behavior.GetName();
        enemyUI = GetComponentInChildren<EnemyUI>();
        enemyUI.SetName();
        target = GameObject.FindWithTag("Player").transform;
        meleeCollider = Weapon.GetComponent<Collider2D>();
        meleeRenderer = Weapon.GetComponent<SpriteRenderer>();

        TimeManger.OnNightStateChanged += HandleNightStateChanged;
        behavior?.OnNightChange(this, TimeManger.GetInstance().isLights);
        TimeManger.OnTimeStop += Freeze;
        TimeManger.OnTimeResume += Unfreeze;
    }

    void Update()
    {
        if (isFrozen) return;

        behavior.Execute(this);

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

    public void ChangeDamage(float dmg)
    {
        damage += dmg;
        enemyWeapon.damage = dmg;
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
        
        Vector2 origin = transform.position;
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float maxDistance = Random.Range(1f, 7f);

        RaycastHit2D hit = Physics2D.Raycast(origin, randomDirection, maxDistance, obstaclesLayer);

        float distanceToMove = hit.collider != null ? hit.distance : maxDistance;
        Vector2 destination = origin + randomDirection * distanceToMove;

        lastSeenPosition = destination;
        moveDirection = destination - origin;

        animator.SetFloat("DirectionX", moveDirection.x);
        animator.SetFloat("DirectionY", moveDirection.y);
    }
    public void Attack()
    {
        isAttacking = true;
        meleeCollider.enabled = true;
        meleeRenderer.enabled = true;
        //animator.SetTrigger("Attack");
        animator.CrossFade("Attack", 0.1f);
        lastAttackTime = Time.time;
        behavior?.OnDamage(this);
        StartCoroutine(ResetWeapon());
        StartCoroutine(ResetAttackState());
    }


    IEnumerator ResetWeapon()
    {
        yield return new WaitForSeconds(0.1f);
        meleeCollider.enabled = false;
        meleeRenderer.enabled = false;
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(0.4f);
        behavior?.OnDeath(this);
        PlayerStats.Instance.GetExpReward(level);
        Destroy(gameObject);
    }
    IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(attackAnimDuration);

        isAttacking = false;
        if (!isAttacking)
        {
            //rb.constraints = RigidbodyConstraints2D.None; 
            //rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
        }
    }


    public void SetBehavior(IEnemyBehavior newBehavior)
    {
        behavior = newBehavior;
    }

    public void TakeDamage(float damage)
    {
        behavior?.OnHurt(this);
        animator.CrossFade("Hurt", 0.1f);
        HP -= damage;
        HP = Mathf.Round(HP * 10f) / 10f;
        enemyUI.SetName();
        if (HP <= 0)
        {
            animator.CrossFade("Death", 0.1f);
            StartCoroutine(Death());
        }
    }

    private void HandleNightStateChanged(bool isNight)
    {
        behavior?.OnNightChange(this, isNight);
    }

    void Freeze()
    {
        if (this == null || gameObject == null) return;  
        if (animator == null) return;  
        isFrozen = true;
        animator.speed = 0f;
        rb.velocity = Vector2.zero;
    }

    void Unfreeze()
    {
        isFrozen = false;
        if (this == null || gameObject == null) return;  
        if (animator == null) return;  
        animator.speed = 1f;
    }
    ~EnemyAI()
    {
        TimeManger.OnNightStateChanged -= HandleNightStateChanged;
        TimeManger.OnTimeStop -= Freeze;
        TimeManger.OnTimeResume -= Unfreeze;
        enemyWeapon.HittingPlayer -= HitPlayer;
    }

    public void HitPlayer()
    {
        behavior?.OnHit(this);
    }
}
 
