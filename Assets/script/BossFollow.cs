using UnityEngine;

namespace Assets.script
{
    public class BossFollow : EnemyFollow
    {
        protected override void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(2); 
                    Debug.Log("Boss đấm Player 2 máu!");
                }
            }
        }
    }
}