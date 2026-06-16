using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("Boss Setup")]
    public GameObject bossPrefab;    
    public Transform spawnPoint;     

    [Header("Spawn Settings")]
    public float timeToSpawn = 60f; 

    private float timer = 0f;
    private bool hasSpawned = false; 

    void Update()
    {
        if (hasSpawned) return;

        timer += Time.deltaTime;

        if (timer >= timeToSpawn)
        {
            SpawnBoss();

            hasSpawned = true;
        }
    }

    void SpawnBoss()
    {
        if (bossPrefab != null && spawnPoint != null)
        {
            Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log("Đã đủ 60s. Boss Bã Mía xuất hiện!!"); 
        }
        else
        {
            Debug.LogWarning("Chưa kéo Boss Prefab hoặc Spawn Point vào BossSpawner kìa ông ơi!");
        }
    }
}