using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed = 5f;     // Tốc độ bay của bã mía
    public int damage = 1;       // Sát thương
    public float lifeTime = 3f;  // Sau 3 giây tự biến mất để đỡ lag game

    private Vector2 moveDirection;

    void Start()
    {
        Destroy(gameObject, lifeTime); // Tự hủy
    }

    // Hàm này để nhận hướng bay từ con Boss truyền sang
    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    void Update()
    {
        // Bay thẳng theo hướng đã định
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu chạm vào Player thì trừ máu
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject); // Chạm mục tiêu thì vỡ đạn
        }
    }
}