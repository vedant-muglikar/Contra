using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stopDistance = 1.5f;
    public int health = 1;

    private Transform player;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;

    private bool stopped = false;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Lion naturally faces LEFT
        spriteRenderer.flipX = false;
    }

    void FixedUpdate()
    {
        if (stopped)
        {
            body.linearVelocity = new Vector2(
                0f,
                body.linearVelocity.y
            );

            return;
        }

        if (player == null)
            return;

        float distance =
            Mathf.Abs(player.position.x - transform.position.x);

        if (distance <= stopDistance)
        {
            body.linearVelocity = new Vector2(
                0f,
                body.linearVelocity.y
            );

            return;
        }

        float direction = 0f;

        if (player.position.x < transform.position.x)
        {
            direction = -1f;
        }
        else if (player.position.x > transform.position.x)
        {
            direction = 1f;
        }

        body.linearVelocity = new Vector2(
            direction * moveSpeed,
            body.linearVelocity.y
        );

        // Lion naturally faces LEFT
        if (direction < 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (direction > 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    public void StopEnemy()
    {
        stopped = true;

        body.linearVelocity = new Vector2(
            0f,
            body.linearVelocity.y
        );

        Debug.Log(
            gameObject.name + " stopped because Level 2 started."
        );
    }

    public void TakeDamage(int damage)
    {
        if (stopped)
            return;

        health -= damage;

        Debug.Log(
            gameObject.name +
            " Health: " +
            health
        );

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");

        EnemyKillManager killManager =
            FindFirstObjectByType<EnemyKillManager>();

        if (killManager != null)
        {
            killManager.EnemyKilled();
        }
        else
        {
            Debug.LogError(
                "EnemyKillManager not found!"
            );
        }

        Destroy(gameObject);
    }
}