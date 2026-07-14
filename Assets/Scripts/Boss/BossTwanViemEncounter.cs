using UnityEngine;
using UnityEngine.UI;

public class BossTwanViemEncounter : MonoBehaviour
{
    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public Transform spawnPoint;

    [Header("Intro UI")]
    public GameObject introPanel;
    public Image introImage;
    public Sprite bossSprite;

    [Header("References")]
    public PlayerExperience playerExperience;

    private bool triggered = false;
    private bool waitingClick = false;

    void Start()
    {
        if (introPanel != null)
            introPanel.SetActive(false);
    }

    void Update()
    {
        // Kiểm tra level 10
        if (!triggered && playerExperience != null && playerExperience.currentLevel >= 10)
        {
            ShowIntro();
        }

        // Chờ click
        if (waitingClick && Input.GetMouseButtonDown(0))
        {
            SpawnBoss();
        }
    }

    void ShowIntro()
    {
        triggered = true;
        waitingClick = true;

        if (introPanel != null)
        {
            introPanel.SetActive(true);
            if (introImage != null && bossSprite != null)
                introImage.sprite = bossSprite;
        }

        Time.timeScale = 0f;
    }

    void SpawnBoss()
    {
        waitingClick = false;

        if (introPanel != null)
            introPanel.SetActive(false);

        Time.timeScale = 1f;

        if (bossPrefab != null && spawnPoint != null)
        {
            GameObject boss = Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);

            // Kết nối với health bar
            BossHealthBarUI healthBar = FindObjectOfType<BossHealthBarUI>();
            if (healthBar != null)
            {
                EnemyHealth bossHealth = boss.GetComponent<EnemyHealth>();
                healthBar.ShowHealthBar(bossHealth);
            }
        }
    }
}
