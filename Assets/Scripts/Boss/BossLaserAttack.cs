using UnityEngine;
public class BossLaserAttack : MonoBehaviour
{
    [Header("References")]
    public GameObject laserBallPrefab;   // Kéo prefab quả cầu to (có thể dùng chung prefab BossBullet cũ, chỉ đổi sprite/scale)
    public Transform firePoint;

    [Header("Timing")]
    [Tooltip("Khoảng cách thời gian giữa 2 lần bắn laser")]
    public float fireInterval = 6f;
    [Tooltip("Thời gian đứng yên sạc chiêu trước khi bắn (để Player thấy telegraph và né)")]
    public float chargeTime = 1f;

    [Header("Đạn quả cầu (ghi đè lên giá trị mặc định trong BossBullet)")]
    public float ballSpeed = 2.5f;   // Chậm hơn hẳn đạn thường
    public int ballDamage = 3;       // Damage cao hơn đạn thường
    public float ballScale = 2f;     // To hơn đạn thường (nhân theo scale gốc của prefab)

    private float timer;
    private Transform player;
    private Animator animator;
    private EnemyFollow follow;
    private bool isBusy = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        follow = GetComponent<EnemyFollow>();

        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target != null) player = target.transform;

        timer = fireInterval; // tránh bắn ngay lúc vừa xuất hiện
    }

    void Update()
    {
        if (player == null || isBusy) return;

        timer += Time.deltaTime;
        if (timer >= fireInterval)
        {
            timer = 0f;
            StartCoroutine(ChargeAndShootRoutine());
        }
    }

    System.Collections.IEnumerator ChargeAndShootRoutine()
    {
        isBusy = true;

        // 1. Sạc chiêu: đứng yên, quay mặt về Player, phát animation "sạc"
        if (follow != null) follow.SetMovementEnabled(false);
        if (player != null) FaceDirection(player.position.x - transform.position.x);

        if (animator != null) animator.SetBool("IsCharging", true);

        yield return new WaitForSeconds(chargeTime);

        if (animator != null) animator.SetBool("IsCharging", false);

        // 2. Bắn: trigger animation bắn + spawn quả cầu về ĐÚNG vị trí Player lúc này
        if (animator != null) animator.SetTrigger("Shoot");
        Shoot();

        // 3. Trả lại quyền đuổi theo cho EnemyFollow
        if (follow != null) follow.SetMovementEnabled(true);

        isBusy = false;
    }

    void Shoot()
    {
        if (laserBallPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Ê ông ơi, quên kéo quả cầu hoặc nòng bắn vào BossLaserAttack rồi!");
            return;
        }

        GameObject ballObj = Instantiate(laserBallPrefab, firePoint.position, Quaternion.identity);
        ballObj.transform.localScale *= ballScale;

        Vector2 targetDirection = player != null
            ? (player.position - firePoint.position).normalized
            : (Vector2)firePoint.right;

        BossBullet ballScript = ballObj.GetComponent<BossBullet>();
        if (ballScript != null)
        {
            // Ghi đè tốc độ/damage để quả cầu chậm và mạnh hơn đạn thường
            ballScript.speed = ballSpeed;
            ballScript.damage = ballDamage;
            ballScript.SetDirection(targetDirection);
        }
    }

    private void FaceDirection(float dirX)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;
        if (dirX > 0.01f) sr.flipX = false;
        else if (dirX < -0.01f) sr.flipX = true;
    }
}