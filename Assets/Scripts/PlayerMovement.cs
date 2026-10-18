using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isGrounded;
    private bool isDropping;

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

        // Flip
        if (move < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (move > 0)
        {
            spriteRenderer.flipX = false;
        }

        // Run animation
        animator.SetBool("run", move != 0);

        // Jump
        if (Keyboard.current.spaceKey.wasPressedThisFrame &&
            isGrounded &&
            !isDropping)
        {
            body.linearVelocity = new Vector2(
                body.linearVelocity.x,
                jumpForce
            );

            isGrounded = false;
        }

        // Drop down
        if (Keyboard.current.downArrowKey.wasPressedThisFrame &&
            isGrounded &&
            !isDropping)
        {
            StartCoroutine(DropDown());
        }

        // Jump animation
        animator.SetBool("grounded", isGrounded);

        // Shoot
        if (Keyboard.current.jKey.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        if (isDropping)
            return;

        float move = 0f;

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

        body.linearVelocity = new Vector2(
            move * moveSpeed,
            body.linearVelocity.y
        );
    }

    // =========================
    // DROP DOWN
    // =========================

    IEnumerator DropDown()
    {
        isDropping = true;
        isGrounded = false;

        animator.SetBool("grounded", false);

        Collider2D playerCollider = GetComponent<Collider2D>();

        // Find the platform we are standing on
        Collider2D[] allColliders =
            FindObjectsByType<Collider2D>(
                FindObjectsSortMode.None
            );

        Collider2D platformToIgnore = null;

        float closestDistance = Mathf.Infinity;

        foreach (Collider2D col in allColliders)
        {
            if (!col.CompareTag("Platform"))
                continue;

            // Check if this platform is underneath the player
            if (col.bounds.max.y <= transform.position.y + 0.2f &&
                col.bounds.max.y >= transform.position.y - 1f)
            {
                float distance = Mathf.Abs(
                    col.bounds.max.y - transform.position.y
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    platformToIgnore = col;
                }
            }
        }

        // Ignore ONLY the platform we're standing on
        if (platformToIgnore != null)
        {
            Physics2D.IgnoreCollision(
                playerCollider,
                platformToIgnore,
                true
            );
        }

        // Move downward
        body.linearVelocity = new Vector2(
            body.linearVelocity.x,
            -5f
        );

        // Wait until player has moved below the platform
        float startY = transform.position.y;

        while (transform.position.y > startY - 1f)
        {
            yield return null;
        }

        // Wait a little extra so we are completely below it
        yield return new WaitForSeconds(0.1f);

        // Re-enable collision
        if (platformToIgnore != null)
        {
            Physics2D.IgnoreCollision(
                playerCollider,
                platformToIgnore,
                false
            );
        }

        isDropping = false;
    }

    // =========================
    // SHOOT
    // =========================

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        float direction =
            spriteRenderer.flipX ? -1f : 1f;

        Bullet bulletScript =
            bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);
        }

        Collider2D bulletCollider =
            bullet.GetComponent<Collider2D>();

        Collider2D playerCollider =
            GetComponent<Collider2D>();

        if (bulletCollider != null &&
            playerCollider != null)
        {
            Physics2D.IgnoreCollision(
                bulletCollider,
                playerCollider
            );
        }
    }

    // =========================
    // GROUND DETECTION
    // =========================

    private void OnCollisionStay2D(
        Collision2D collision
    )
    {
        if (isDropping)
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;

                animator.SetBool(
                    "grounded",
                    true
                );

                return;
            }
        }
    }

    private void OnCollisionExit2D(
        Collision2D collision
    )
    {
        // Don't change grounded here.
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameOverManager gameOverManager =
                FindFirstObjectByType<GameOverManager>();

            if (gameOverManager != null)
            {
                gameOverManager.GameOver();
            }
        }
    }
}