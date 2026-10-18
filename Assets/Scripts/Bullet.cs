using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 1;
    public float lifetime = 3f;

    private float direction = 1f;
    private Rigidbody2D body;

    public void SetDirection(float newDirection)
    {
        direction = newDirection;
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();

        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (body != null)
        {
            body.linearVelocity = new Vector2(
                direction * speed,
                0f
            );
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(
            "Player bullet hit: " +
            collision.gameObject.name
        );

        // Level 2 enemy
        Level2Enemy level2Enemy =
            collision.gameObject.GetComponent<Level2Enemy>();

        if (level2Enemy == null)
        {
            level2Enemy =
                collision.gameObject.GetComponentInParent<Level2Enemy>();
        }

        if (level2Enemy != null)
        {
            Debug.Log("🔴 Player bullet hit Level 2 Enemy!");

            level2Enemy.TakeDamage(damage);

            Destroy(gameObject);

            return;
        }

        // Level 1 enemy
        Enemy enemy =
            collision.gameObject.GetComponent<Enemy>();

        if (enemy == null)
        {
            enemy =
                collision.gameObject.GetComponentInParent<Enemy>();
        }

        if (enemy != null)
        {
            Debug.Log("🟢 Player bullet hit Level 1 Enemy!");

            enemy.TakeDamage(damage);

            Destroy(gameObject);

            return;
        }

        // Bullet hit something else
        Destroy(gameObject);
    }
}