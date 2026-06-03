using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;

    private int currentHealth;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        healthBar.SetMaxHealth(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);

        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("GAME OVER - Chạy UI chết!");

        // Báo cáo cho tổng tư lệnh GameManager biết để dừng hình, bật bảng UI và đổi nhạc
        if (GameManager.instance != null)
        {
            GameManager.instance.PlayerDied();
        }

        // Tắt (ẩn) nhân vật đi thay vì Destroy để tránh bị lỗi văng game do Camera/Quái mất mục tiêu
        gameObject.SetActive(false);
    }
}