using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;

    [Header("Spawn Settings")]
    public Transform spawnPoint;

    public float spawnDelay = 2f;

    private GameObject currentEnemy;

    void Start()
    {
        SpawnEnemy();
    }

    void Update()
    {
        if (currentEnemy == null)
        {
            spawnDelay -= Time.deltaTime;

            if (spawnDelay <= 0)
            {
                SpawnEnemy();
                spawnDelay = 2f;
            }
        }
    }

    void SpawnEnemy()
    {
        currentEnemy = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            Quaternion.identity
        );
    }
}
