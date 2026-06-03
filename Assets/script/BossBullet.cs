using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 7f;      // Tốc độ bay của đạn
    public int damage = 1;        // Sát thương gây ra cho Player
    public float lifeTime = 3f;   // Sau 3 giây không trúng ai sẽ tự hủy để đỡ lag game

    private Vector2 moveDirection;

    void Start()
    {
        // Vừa đẻ ra là kích hoạt hẹn giờ tự hủy luôn
        Destroy(gameObject, lifeTime);
    }

    // Hàm này để con Boss gọi và truyền hướng bay cho viên đạn
    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;

        if (moveDirection != Vector2.zero)
        {
            // ẢNH NGANG THÌ KHÔNG TRỪ 90 NỮA! Để mộc mạc thế này thôi
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;

            // Xoay góc Z của viên đạn theo đúng hướng bay
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu đạn chạm trúng Player
        if (collision.CompareTag("Player"))
        {
            // Lấy script máu của Player ra để trừ máu
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Player dính đạn bã mía, mất " + damage + " máu!");
            }

            // Gây sát thương xong thì hủy viên đạn
            Destroy(gameObject);
        }
    }
}