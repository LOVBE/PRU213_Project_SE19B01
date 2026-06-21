using UnityEngine;
public class EnemyFollow : MonoBehaviour
{
    public float moveSpeed = 2f;
    private float baseMoveSpeed;

    private bool isSlowed = false;
    private float slowEndTime = 0f;
    private float slowFactor = 1f;

    private Transform player;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    // Cho phép script khác (vd BossDash) tạm "chiếm quyền" di chuyển khỏi EnemyFollow
    private bool movementEnabled = true;
    void Awake()
    {
        baseMoveSpeed = moveSpeed;
    }

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
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Nếu đang bị skill khác (vd Dash) tạm khoá thì bỏ qua logic bám đuổi bình thường
        if (!movementEnabled) return;
        if (isSlowed && Time.time >= slowEndTime)
        {
            isSlowed = false;
            moveSpeed = baseMoveSpeed;
            if (sr != null) sr.color = Color.white;
        }

        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            if (player.position.x > transform.position.x)
            {
                sr.flipX = false;
            }
            else if (player.position.x < transform.position.x)
            {
                sr.flipX = true;
            }
        }
    }

    // Gọi từ script khác để bật/tắt khả năng tự bám đuổi (vd lúc Boss đang Dash)
    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    public void ApplySlow(float duration, float factor)
    {
        isSlowed = true;
        slowEndTime = Time.time + duration;
        slowFactor = Mathf.Clamp(factor, 0.05f, 1f);
        moveSpeed = baseMoveSpeed * slowFactor;

        if (sr != null)
        {
            PlayerHealth playerHealth =
                collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
            sr.color = new Color(0.5f, 0.8f, 1f);
        }
    }
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        TryDamagePlayer(collision.gameObject);
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other.gameObject);
    }

    void TryDamagePlayer(GameObject obj)
    {
        if (!obj.CompareTag("Player")) return;

        PlayerHealth playerHealth = obj.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(1);
        }
    }
}