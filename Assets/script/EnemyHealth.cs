using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Hàm nhận damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Enemy HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Enemy chết
    void Die()
    {
        Destroy(gameObject);
    }
}