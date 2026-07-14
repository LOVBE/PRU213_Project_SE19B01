using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 7f;     
    public int damage = 1;       
    public float lifeTime = 3f;   

    private Vector2 moveDirection;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        Debug.Log($"Đạn nhận hướng: {moveDirection}, độ lớn: {moveDirection.magnitude}");

        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Player dính đạn bã mía, mất " + damage + " máu!");
            }

            Destroy(gameObject);
        }
    }
}