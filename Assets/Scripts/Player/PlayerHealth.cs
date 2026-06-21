using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance; 

    public int maxHealth = 5;
    public int currentHealth;
    public HealthBar healthBar;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("HasSave", 0) == 1)
        {
            currentHealth = PlayerPrefs.GetInt("SavedHP", maxHealth);
        }
        else
        {
            currentHealth = maxHealth;
        }

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
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

        PlayerPrefs.DeleteKey("HasSave");
        PlayerPrefs.DeleteKey("SavedHP");
        PlayerPrefs.DeleteKey("LastScene");
        PlayerPrefs.Save();

        if (GameManager.instance != null)
            GameManager.instance.PlayerDied();

        gameObject.SetActive(false);
    }
    public void SaveData()
    {
        PlayerPrefs.SetInt(
            "SavedHP",
            currentHealth);
    }
}