using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovment : MonoBehaviour
{

    [Header("Movement Params")]
    public float moveSpeed = 6.0f;

    private CapsuleCollider2D coll;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 lastMove;
    private Vector2 cursor;
    public float dodgeSpeed = 12.0f; 
    public float dodgeDuration = 0.3f;
    private bool isDodging = false;
    private float dodgeTimer = 0f;
    public Transform Aim;
    private static PlayerMovment instance;
    bool isWalking = false;

    private void Awake()
    {
        instance = this;
        coll = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        animator = GetComponent<Animator>();
    }

    public static PlayerMovment GetInctance()
    {
        return instance;
    }

    private void FixedUpdate()
    {
        if ((DialogueManager.GetInstance() == null || !DialogueManager.GetInstance().getDialogueIsPlaying()) && !PlayerAttack.GetInstance().GetAttacking())
        {
            moveSpeed = 6f;
            HandleMovement();
        }
        else
        {
            moveSpeed = 0f;
        }

    }

    private void HandleMovement()
    {
        Vector2 moveDirection = InputManager.GetInstance().GetMoveDirection();
        rb.velocity = moveDirection * moveSpeed;
        
        UpdateAnimator(moveDirection);
    }

    private void UpdateAnimator(Vector2 moveDirection)
    {
        if(animator!= null)
        {
            animator.SetFloat("Speed", moveDirection.sqrMagnitude);
            if (moveDirection.x != 0 || moveDirection.y != 0)
            {
                isWalking = true;
                lastMove.x = moveDirection.x;
                lastMove.y = moveDirection.y;
            }
            else
            {
                isWalking = false;
            }
            Vector3 v3 = Vector3.left * lastMove.x + Vector3.down * lastMove.y;
            Aim.rotation = Quaternion.LookRotation(Vector3.forward, v3);

            cursor.x = Aim.position.x;
            cursor.y = Aim.position.y;

            animator.SetFloat("DirectionX", lastMove.x);
            animator.SetFloat("DirectionY", lastMove.y);

        }
    }

    public void Dodge()
    {
        if (isDodging) return; 

        isDodging = true;
        dodgeTimer = dodgeDuration;

        Vector2 dodgeDirection = InputManager.GetInstance().GetMoveDirection().normalized;
        rb.velocity = dodgeDirection * dodgeSpeed;

        if (animator != null)
        {
            animator.SetTrigger("Dodge");
        }
    }

    private void HandleDodge()
    {
        if (!isDodging) return;

        dodgeTimer -= Time.fixedDeltaTime;
        if (dodgeTimer <= 0f)
        {
            isDodging = false;
            rb.velocity = Vector2.zero; // —брасываем скорость после уклонени€
        }
    }

    public Vector2 GetPlayerPosition()
    {
        return lastMove;
    }
}
