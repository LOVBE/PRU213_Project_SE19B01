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

    void Start()
    {
        // Nếu có save thì spawn đúng số lượng đã lưu
        if (PlayerPrefs.GetInt("HasSave", 0) == 1)
        {
            int savedCount = PlayerPrefs.GetInt("SavedEnemyCount", 0);
            for (int i = 0; i < savedCount; i++)
                SpawnEnemy();

            // Xóa key sau khi load xong
            PlayerPrefs.DeleteKey("SavedEnemyCount");
        }
    }

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