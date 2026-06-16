using UnityEngine;

public class BossShoot : MonoBehaviour
{
    [Header("References")]
    public GameObject bamiaBulletPrefab; // Kéo Prefab đạn bã mía ở kho Project vào đây
    public Transform firePoint;          // Kéo Object "Mouth" vào đây

    [Header("Shoot Settings")]
    public float fireRate = 2f;          // Tốc độ bắn (cứ 2 giây bắn 1 lần)

    private float timer;
    private Transform player;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        GameObject target = GameObject.FindGameObjectWithTag("Player");

        if (target != null)
        {
            player = target.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Bộ đếm thời gian
        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            animator.SetTrigger("Shoot");

            Shoot();

            timer = 0f;
        }
    }

    void Shoot()
    {
        if (bamiaBulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Ê ông ơi, quên kéo đạn hoặc nòng súng vào script BossShoot rồi!");
            return;
        }

        // 1. Sinh ra viên đạn ngay tại vị trí nòng súng (firePoint)
        GameObject bulletObj = Instantiate(bamiaBulletPrefab, firePoint.position, Quaternion.identity);

        // 2. Tính toán hướng: Lấy vị trí Player trừ vị trí nòng súng để ra vector hướng thẳng vào mặt Player
        Vector2 targetDirection = (player.position - firePoint.position).normalized;

        // 3. Ép viên đạn bay theo hướng vừa tính
        BossBullet bulletScript = bulletObj.GetComponent<BossBullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(targetDirection);
        }
    }
}