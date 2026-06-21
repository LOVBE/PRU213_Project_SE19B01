using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;
    public int maxHealth = 5;
    public int currentHealth;
    public HealthBar healthBar;

    private Rigidbody2D rb;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (PlayerPrefs.GetInt("HasSave", 0) == 1)
        {
            currentHealth = PlayerPrefs.GetInt("SavedHP", maxHealth);

            // Khôi phục lại vị trí Player đã lưu trước đó
            if (PlayerPrefs.HasKey("SavedPosX"))
            {
                float x = PlayerPrefs.GetFloat("SavedPosX");
                float y = PlayerPrefs.GetFloat("SavedPosY");
                float z = PlayerPrefs.GetFloat("SavedPosZ");
                Vector3 savedPos = new Vector3(x, y, z);

                transform.position = savedPos;

                // Nếu có Rigidbody2D, đặt luôn vị trí cho rb để tránh bị
                // FixedUpdate (rb.MovePosition) kéo về vị trí cũ
                if (rb != null)
                {
                    rb.position = savedPos;
                }
            }
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

        // Xoá toàn bộ dữ liệu save khi chết, để lần chơi mới bắt đầu từ đầu
        PlayerPrefs.DeleteKey("HasSave");
        PlayerPrefs.DeleteKey("SavedHP");
        PlayerPrefs.DeleteKey("LastScene");
        PlayerPrefs.DeleteKey("SavedPosX");
        PlayerPrefs.DeleteKey("SavedPosY");
        PlayerPrefs.DeleteKey("SavedPosZ");
        PlayerPrefs.DeleteKey("SavedLevel");
        PlayerPrefs.DeleteKey("SavedExp");
        PlayerPrefs.DeleteKey("SavedEnemyCount");
        PlayerPrefs.DeleteKey("SavedBossAlive");
        PlayerPrefs.Save();

        if (GameManager.instance != null)
            GameManager.instance.PlayerDied();
        gameObject.SetActive(false);
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("SavedHP", currentHealth);

        Vector3 pos = transform.position;
        PlayerPrefs.SetFloat("SavedPosX", pos.x);
        PlayerPrefs.SetFloat("SavedPosY", pos.y);
        PlayerPrefs.SetFloat("SavedPosZ", pos.z);
    }
}