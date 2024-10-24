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
    private float MoveX, MoveY = 0;
    private void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (DialogueManager.GetInstance() == null)
        {
            moveSpeed = 6f;
            HandleMovement();
        }
        if (DialogueManager.GetInstance().getDialogueIsPlaying())
        {
            moveSpeed = 0f;
            //return;
        }
        else
        {
            moveSpeed = 6f;
        }

        HandleMovement();
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
            if(moveDirection.x!=0 || moveDirection.y !=0)
            {
                MoveX = moveDirection.x;
                MoveY = moveDirection.y;
            }
            animator.SetFloat("DirectionX", MoveX);
            animator.SetFloat("DirectionY", MoveY);

        }
    }
}
