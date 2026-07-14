using UnityEngine;
namespace Assets.script
{
    public class BossFollow : EnemyFollow
    {
        [Header("Boss Follow Settings")]
        public float bossMoveSpeed = 2f;

        [Header("Damage Settings")]
        public int normalDamage = 2;

        private Transform player;
        private SpriteRenderer sr;
        private Rigidbody2D rb;
        private bool movementEnabled = true;
        private BossDash bossDash;

        new void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;

            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
            bossDash = GetComponent<BossDash>();
        }

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