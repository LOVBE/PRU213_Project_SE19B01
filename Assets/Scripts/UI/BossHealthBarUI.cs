using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject healthBarPanel;
    public Image healthBarFill;
    public Text bossNameText;

    private EnemyHealth currentBoss;

    void Start()
    {
        if (healthBarPanel != null)
            healthBarPanel.SetActive(false);
    }

    public void ShowHealthBar(EnemyHealth boss)
    {
        currentBoss = boss;

        if (healthBarPanel != null)
            healthBarPanel.SetActive(true);

        if (bossNameText != null)
            bossNameText.text = "Twan Viem";

        UpdateHealthBar();
    }

    void Update()
    {
        if (currentBoss != null)
        {
            UpdateHealthBar();

            // Ẩn khi boss chết
            if (currentBoss.CurrentHealth <= 0)
            {
                HideHealthBar();
            }
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null && currentBoss != null)
        {
            float fillAmount = (float)currentBoss.CurrentHealth / currentBoss.MaxHealth;
            healthBarFill.fillAmount = fillAmount;
        }
    }

    void HideHealthBar()
    {
        if (healthBarPanel != null)
            healthBarPanel.SetActive(false);

        currentBoss = null;
    }
}
