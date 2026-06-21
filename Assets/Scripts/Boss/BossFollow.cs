using UnityEngine;
namespace Assets.script
{
    public class BossFollow : EnemyFollow
    {
        [Header("Damage Settings")]
        public int normalDamage = 2;    

        private BossDash bossDash;

        protected override void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // Lấy component BossDash khi cần (lazy), không đụng tới Start() của lớp cha
                if (bossDash == null) bossDash = GetComponent<BossDash>();

                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    // Nếu đang trong lúc Dash thì gây damage cao hơn (lấy từ BossDash.dashDamage)
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