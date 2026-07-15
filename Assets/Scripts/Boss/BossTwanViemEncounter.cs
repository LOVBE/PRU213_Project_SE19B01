using UnityEngine;
using UnityEngine.UI;

public class BossTwanViemEncounter : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("Level yêu cầu để boss này xuất hiện. Đổi số này cho từng boss khác nhau (vd Twan Viem = 10, boss khác = 18)")]
    public int requiredLevel = 10;

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
        // Kiểm tra level yêu cầu (khác nhau tuỳ boss, cấu hình qua requiredLevel)
        if (!triggered && playerExperience != null && playerExperience.currentLevel >= requiredLevel)
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