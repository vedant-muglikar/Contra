using UnityEngine;

public class EnemyKillManager : MonoBehaviour
{
    [Header("Kill Settings")]
    public int enemiesRequired = 20;

    [Header("Bridge")]
    public GameObject bridge;

    [Header("Enemy Spawner")]
    public EnemySpawner enemySpawner;

    private int enemiesKilled = 0;

    private bool bridgeOpened = false;

    void Start()
    {
        // Close bridge at the beginning
        if (bridge != null)
        {
            bridge.SetActive(false);
        }
        else
        {
            Debug.LogError(
                "❌ Bridge is NOT assigned!"
            );
        }
    }

    public void EnemyKilled()
    {
        // Don't count anything after bridge opens
        if (bridgeOpened)
            return;

        enemiesKilled++;

        Debug.Log(
            "🔥 Enemies Killed: " +
            enemiesKilled +
            "/" +
            enemiesRequired
        );

        if (enemiesKilled >= enemiesRequired)
        {
            OpenBridge();
        }
    }

    void OpenBridge()
    {
        if (bridgeOpened)
            return;

        bridgeOpened = true;

        // Open bridge
        if (bridge != null)
        {
            bridge.SetActive(true);

            Debug.Log(
                "🌉 BRIDGE OPENED!"
            );
        }
        else
        {
            Debug.LogError(
                "Bridge reference is missing!"
            );
        }

        // Start Level 2
        if (enemySpawner != null)
        {
            enemySpawner.StartLevel2();
        }
        else
        {
            Debug.LogError(
                "❌ EnemySpawner is NOT assigned!"
            );
        }
    }
}