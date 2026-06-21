using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("Boss Setup")]
    public GameObject bossPrefab;
    public Transform spawnPoint;

    [Header("Spawn Settings")]
    public float timeToSpawn = 60f;
    public int requiredLevel = 4;
    public PlayerExperience player;

    private float timer = 0f;
    private bool hasSpawned = false;

    void Start()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerExperience>();
        }

        // Nếu có save và boss còn sống → spawn ngay, bỏ qua timer
        if (PlayerPrefs.GetInt("HasSave", 0) == 1)
        {
            bool bossWasAlive = PlayerPrefs.GetInt("SavedBossAlive", 0) == 1;
            if (bossWasAlive)
            {
                SpawnBoss();
                hasSpawned = true;
            }
            else
            {
                // Boss đã chết trước khi save → không spawn lại
                hasSpawned = true;
            }

            PlayerPrefs.DeleteKey("SavedBossAlive");
        }
    }

    void Update()
    {
        if (hasSpawned) return;

        if (player == null || player.currentLevel < requiredLevel) return;

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