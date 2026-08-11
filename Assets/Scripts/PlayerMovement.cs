using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isGrounded;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float move = 0f;

        // Movement
        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            move = -1f;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            move = 1f;
        }

        // Move
        body.linearVelocity = new Vector2(
            move * moveSpeed,
            body.linearVelocity.y
        );

        //Flip
        if (move < 0)
            spriteRenderer.flipX = true;
        else if (move > 0)
            spriteRenderer.flipX = false;

        // Run animation
        animator.SetBool("run", move != 0);

        // Jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            body.linearVelocity = new Vector2(
                body.linearVelocity.x,
                jumpForce
            );

            isGrounded = false;
        }

        // Jump animation
        animator.SetBool("grounded", isGrounded);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                if (!isGrounded)
                {
                    isGrounded = true;

                    // Immediately stop jump animation
                    if (animator != null)
                    {
                        if (Mathf.Abs(body.linearVelocity.x) > 0.01f)
                            animator.Play("run");
                        else
                            animator.Play("idle");
                    }
                }

                return;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}