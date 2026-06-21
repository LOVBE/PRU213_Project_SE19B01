using UnityEngine;
namespace Assets.script
{
    public class BossFollow : EnemyFollow
    {
        [Header("Boss Follow Settings")]
        public float bossMoveSpeed = 2f; // Tốc độ bám đuổi riêng của Boss (đổi tên để không trùng field moveSpeed của EnemyFollow)

        [Header("Damage Settings")]
        public int normalDamage = 2; // Damage khi va chạm bình thường (không dash)

        private Transform player;
        private SpriteRenderer sr;
        private Rigidbody2D rb;
        private bool movementEnabled = true;
        private BossDash bossDash;

        // Dùng "new" để KHÔNG đụng/đè vào Start() gốc của EnemyFollow, Boss tự quản lý riêng
        new void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;

            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            bossDash = GetComponent<BossDash>();
        }

        // Dùng "new" để Boss tự di chuyển riêng, không dùng FixedUpdate gốc của EnemyFollow
        new void FixedUpdate()
        {
            if (!movementEnabled) return;
            if (player == null || rb == null) return;

            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * bossMoveSpeed * Time.fixedDeltaTime);

            if (sr != null)
            {
                if (player.position.x > transform.position.x) sr.flipX = false;
                else if (player.position.x < transform.position.x) sr.flipX = true;
            }
        }

        // Gọi từ BossDash để tạm khoá/bật lại khả năng tự bám đuổi của Boss
        public void SetMovementEnabled(bool enabled)
        {
            movementEnabled = enabled;
        }

        protected override void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (bossDash == null) bossDash = GetComponent<BossDash>();

                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    // Nếu đang trong lúc Dash thì gây damage cao hơn
                    if (bossDash != null && bossDash.IsDashing)
                    {
                        playerHealth.TakeDamage(bossDash.dashDamage);
                        Debug.Log("Boss DASH trúng Player, mất " + bossDash.dashDamage + " máu!");
                    }
                    else
                    {
                        playerHealth.TakeDamage(normalDamage);
                        Debug.Log("Boss đấm Player " + normalDamage + " máu!");
                    }
                }
            }
        }
    }
}