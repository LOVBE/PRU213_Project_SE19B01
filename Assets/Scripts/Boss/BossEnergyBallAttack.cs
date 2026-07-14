using UnityEngine;
using System.Collections;
using Assets.script;

public class BossEnergyBallAttack : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Kéo prefab CreepBullet hoặc energy ball vào đây")]
    public GameObject energyBallPrefab;

    [Tooltip("Vị trí bắn đạn (tạo Empty GameObject con làm firePoint)")]
    public Transform firePoint;

    [Header("Timing")]
    [Tooltip("Thời gian giữa 2 lần bắn (giây)")]
    public float attackInterval = 5f;

    [Tooltip("Thời gian vận chiêu trước khi bắn (giây)")]
    public float telegraphTime = 1.5f;

    [Header("Energy Ball Settings")]
    [Tooltip("Tốc độ của quả cầu")]
    public float ballSpeed = 4f;

    [Tooltip("Damage của mỗi quả cầu")]
    public int ballDamage = 2;

    [Tooltip("Kích thước quả cầu (nhân với scale gốc)")]
    public float ballScale = 1.5f;

    [Tooltip("Góc lệch của viên trên/dưới so với viên giữa (độ)")]
    public float spreadAngle = 22.5f;

    private float timer;
    private Transform player;
    private Animator animator;
    private BossFollow bossFollow;
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        bossFollow = GetComponent<BossFollow>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        timer = attackInterval;
    }

    void Update()
    {
        if (player == null || isAttacking)
            return;

        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            timer = 0f;
            StartCoroutine(TelegraphAndShootRoutine());
        }
    }

    IEnumerator TelegraphAndShootRoutine()
    {
        isAttacking = true;

        // 1. Dừng di chuyển và quay về phía player
        if (bossFollow != null)
        {
            bossFollow.SetMovementEnabled(false);
        }

        if (player != null)
        {
            FacePlayer();
        }

        // 2. Phát animation vận chiêu (telegraph)
        if (animator != null)
        {
            animator.SetTrigger("Telegraph");
        }

        yield return new WaitForSeconds(telegraphTime);

        // 3. Bắn 3 quả cầu
        ShootEnergyBalls();

        // 4. Cho phép di chuyển trở lại
        if (bossFollow != null)
        {
            bossFollow.SetMovementEnabled(true);
        }

        isAttacking = false;
    }

    void ShootEnergyBalls()
    {
        if (energyBallPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Thiếu energy ball prefab hoặc fire point!");
            return;
        }

        if (player == null)
            return;

        // Tính hướng bắn về phía player
        Vector2 baseDirection = (player.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;

        // Bắn 3 quả: giữa (thẳng), trên (+góc), dưới (-góc)
        float[] angles = { 0f, spreadAngle, -spreadAngle };

        foreach (float angleOffset in angles)
        {
            float currentAngle = baseAngle + angleOffset;
            Vector2 shootDirection = new Vector2(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad)
            );

            SpawnEnergyBall(shootDirection);
        }
    }

    void SpawnEnergyBall(Vector2 direction)
    {
        GameObject ball = Instantiate(energyBallPrefab, firePoint.position, Quaternion.identity);
        ball.transform.localScale *= ballScale;

        BossBullet bulletScript = ball.GetComponent<BossBullet>();
        if (bulletScript != null)
        {
            bulletScript.speed = ballSpeed;
            bulletScript.damage = ballDamage;
            bulletScript.SetDirection(direction);
        }
    }

    void FacePlayer()
    {
        if (player == null)
            return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float direction = player.position.x - transform.position.x;
            if (direction > 0)
                sr.flipX = false;
            else if (direction < 0)
                sr.flipX = true;
        }
    }
}
