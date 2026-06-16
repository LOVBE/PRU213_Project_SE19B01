using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 2;

    private int currentHealth;

    [Header("Drop EXP")]
    public GameObject expPrefab;

    public int expDropAmount = 1;

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

        Destroy(gameObject);
    }

    void DropExperience()
    {
        if (expPrefab == null)
            return;

        for (int i = 0; i < expDropAmount; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-0.3f, 0.3f),
                Random.Range(-0.3f, 0.3f),
                0);

            Instantiate(
                expPrefab,
                transform.position + offset,
                Quaternion.identity);
        }
    }
}