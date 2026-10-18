using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Level 1 Enemy")]
    public GameObject enemyPrefab;

    [Header("Level 2 Enemy")]
    public GameObject level2EnemyPrefab;

    [Header("Level 1 Spawn Points")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    [Header("Level 2 Spawn Points")]
    public Transform level2LeftSpawnPoint;
    public Transform level2RightSpawnPoint;

    [Header("Spawn Settings")]
    public float spawnDelay = 5f;
    public int maxEnemies = 5;
    public float minimumSpawnDistance = 8f;

    private float timer = 0f;
    private bool level2Started = false;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer < spawnDelay)
            return;

        timer = 0f;

        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        GameObject prefabToSpawn;
        Transform spawnPoint;

        // ==========================
        // LEVEL 1
        // ==========================
        if (!level2Started)
        {
            // Count only Level 1 enemies
            Enemy[] enemies =
                FindObjectsByType<Enemy>(
                    FindObjectsSortMode.None
                );

            if (enemies.Length >= maxEnemies)
            {
                return;
            }

            prefabToSpawn = enemyPrefab;

            if (Random.value < 0.5f)
            {
                spawnPoint = leftSpawnPoint;
            }
            else
            {
                spawnPoint = rightSpawnPoint;
            }
        }

        // ==========================
        // LEVEL 2
        // ==========================
        else
        {
            // Count only Level 2 enemies
            Level2Enemy[] enemies =
                FindObjectsByType<Level2Enemy>(
                    FindObjectsSortMode.None
                );

            if (enemies.Length >= maxEnemies)
            {
                return;
            }

            prefabToSpawn = level2EnemyPrefab;

            if (Random.value < 0.5f)
            {
                spawnPoint = level2LeftSpawnPoint;
            }
            else
            {
                spawnPoint = level2RightSpawnPoint;
            }
        }

        // Check prefab
        if (prefabToSpawn == null)
        {
            Debug.LogError(
                "❌ Enemy prefab is NOT assigned!"
            );

            return;
        }

        // Check spawn point
        if (spawnPoint == null)
        {
            Debug.LogError(
                "❌ Spawn point is NOT assigned!"
            );

            return;
        }

        // Find player
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError(
                "❌ Player with tag 'Player' not found!"
            );

            return;
        }

        // Check distance
        float distance = Vector2.Distance(
            spawnPoint.position,
            playerObject.transform.position
        );

        if (distance < minimumSpawnDistance)
        {
            Debug.Log(
                "⚠️ Spawn point is too close to player: " +
                spawnPoint.name
            );

            return;
        }

        // Spawn
        GameObject newEnemy = Instantiate(
            prefabToSpawn,
            spawnPoint.position,
            Quaternion.identity
        );

        if (level2Started)
        {
            Debug.Log(
                "🔴 LEVEL 2 ENEMY SPAWNED: " +
                newEnemy.name +
                " at " +
                spawnPoint.name
            );
        }
        else
        {
            Debug.Log(
                "🟢 LEVEL 1 ENEMY SPAWNED: " +
                newEnemy.name +
                " at " +
                spawnPoint.name
            );
        }
    }

    public void StartLevel2()
    {
        if (level2Started)
            return;

        level2Started = true;

        // Reset timer so Level 2 starts spawning
        timer = 0f;

        Debug.Log(
            "================================"
        );

        Debug.Log(
            "🔥 LEVEL 2 STARTED!"
        );

        Debug.Log(
            "🔴 Level 2 enemies will now spawn."
        );

        Debug.Log(
            "================================"
        );

        StopLevel1Enemies();
    }

    void StopLevel1Enemies()
    {
        Enemy[] enemies =
            FindObjectsByType<Enemy>(
                FindObjectsSortMode.None
            );

        foreach (Enemy enemy in enemies)
        {
            enemy.StopEnemy();
        }

        Debug.Log(
            "🛑 Stopped " +
            enemies.Length +
            " Level 1 enemies."
        );
    }
}