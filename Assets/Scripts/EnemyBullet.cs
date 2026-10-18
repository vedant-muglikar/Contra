using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    private float direction = 1f;

    public void SetDirection(float newDirection)
    {
        direction = newDirection;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(
            Vector2.right *
            direction *
            speed *
            Time.deltaTime
        );
    }

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        // Hit player
        if (collision.gameObject.CompareTag("Player"))
        {
            GameOverManager gameOverManager =
                FindFirstObjectByType<GameOverManager>();

            if (gameOverManager != null)
            {
                gameOverManager.GameOver();
            }
        }

        // Destroy bullet after collision
        Destroy(gameObject);
    }
}