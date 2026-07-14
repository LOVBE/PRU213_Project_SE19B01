using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    // Các script khác (vd LevelExitTrigger) sẽ lắng nghe event này
    public static event Action OnBossDied;

    public int maxHealth = 2;
    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    [Header("Drop EXP")]
    public GameObject expPrefab;
    public int expDropAmount = 1;

    [Header("Boss Settings")]
    public bool isBoss = false; // Tick ô này trong Inspector nếu object này là Boss

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy HP: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        DropExperience();

        if (isBoss)
        {
            OnBossDied?.Invoke();
        }

        Destroy(gameObject);
    }

    void DropExperience()
    {
        if (expPrefab == null)
            return;

        for (int i = 0; i < expDropAmount; i++)
        {
            Vector3 offset = new Vector3(
    UnityEngine.Random.Range(-0.3f, 0.3f),
    UnityEngine.Random.Range(-0.3f, 0.3f),
    0f
);
            Instantiate(
                expPrefab,
                transform.position + offset,
                Quaternion.identity);
        }
    }
}