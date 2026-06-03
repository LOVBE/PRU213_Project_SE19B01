using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public float moveSpeed = 2f;

    private Transform player;
    private SpriteRenderer sr;
    private Rigidbody2D rb; // 1. Khai báo thêm Rigidbody2D

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Không tìm thấy Player trong Scene!");
        }

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>(); // Lấy component Rigidbody2D của quái
    }

    // 2. Đổi Update thành FixedUpdate (Luật bất thành văn: Đụng đến vật lý di chuyển LÀ PHẢI DÙNG FixedUpdate)
    void FixedUpdate()
    {
        if (player != null)
        {
            // Tính toán hướng đi (Vector chỉ từ quái thẳng vào mặt Player)
            Vector2 direction = (player.position - transform.position).normalized;

            // Dùng MovePosition của Rigidbody để đi. Cái này giúp quái tự biết trượt mượt mà khi chạm tường!
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

            // Đổi hướng nhìn
            if (player.position.x > transform.position.x)
            {
                sr.flipX = false; // nhìn phải
            }
            else if (player.position.x < transform.position.x)
            {
                sr.flipX = true; // nhìn trái
            }
        }
    }

    // Tớ thấy ông đang xài lại OnCollision (Va chạm cứng). 
    // Nhớ là nếu dùng cái này, quái và Player sẽ đẩy nhau côm cốp nhé.
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth =
                collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
    }
}