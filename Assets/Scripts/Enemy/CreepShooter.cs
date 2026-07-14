using UnityEngine;

public class CreepShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float attackRange = 5f;
    public float attackCooldown = 1.5f;

    private Transform player;
    private Animator animator;
    private float nextAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            Debug.Log("TRY ATTACK");
            animator.SetTrigger("shoot");

            Invoke(nameof(Shoot), 0.2f);

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public void Shoot()
    {
        if (player == null || bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Missing references!");
            return;
        }

        Debug.Log("Shooting bullet!");

        Vector3 spawnPos = firePoint.position;
        spawnPos.z = -1f;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Vector2 dir = ((Vector2)player.position - (Vector2)firePoint.position).normalized;

        BossBullet bb = bullet.GetComponent<BossBullet>();
        if (bb != null)
        {
            bb.SetDirection(dir);
            Debug.Log("Bullet created!");
        }
    }
}