using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;

    [Header("Spawn Settings")]
    public Transform spawnPoint;

    public float spawnInterval = 2f;

    public int maxEnemies = 10;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            GameObject[] enemies =
                GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length < maxEnemies)
            {
                SpawnEnemy();
            }

            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        Instantiate(
            enemyPrefab,
            spawnPoint.position,
            Quaternion.identity
        );
    }
}