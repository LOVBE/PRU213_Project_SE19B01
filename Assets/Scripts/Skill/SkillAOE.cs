using UnityEngine;

public class SkillAOE : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 30;

    [Header("Area")]
    public float radius = 4f;

    [Header("Lifetime")]
    public float lifeTime = 0.6f;

    [Header("Optional Slow")]
    public bool applySlow = false;
    public float slowDuration = 2f;
    public float slowFactor = 0.5f;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    [Tooltip("Màu lúc vừa nổ (đầu đời).")]
    public Color startColor = new Color(1f, 0.5f, 0f, 0.7f);

    [Tooltip("Màu lúc sắp biến mất.")]
    public Color endColor = new Color(1f, 0f, 0f, 0f);

    [Tooltip("Tự tăng kích thước theo thời gian để tạo cảm giác nổ ra.")]
    public bool expandOnExplode = true;
    public float startScaleMultiplier = 0.6f;
    public float endScaleMultiplier = 1.1f;

    [Tooltip("Nổ ngay khi spawn (damage 1 phát) hay chờ trigger?")]
    public bool explodeOnSpawn = true;

    [Tooltip("Layer để lọc enemy khi dùng OverlapCircleAll (để trống = tất cả).")]
    public LayerMask enemyMask = ~0;

    private bool hasExploded = false;
    private float spawnTime;
    private Vector3 baseScale;

    void OnEnable()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null && spriteRenderer.sprite == null)
        {
            Debug.LogWarning("[SkillAOE] Prefab '" + name + "' chưa có sprite! SpriteRenderer sẽ không hiển thị gì. Hãy gán 1 sprite (vd. Experience_Orb.png hoặc sprite tròn trắng) trong Inspector.");
        }

        baseScale = new Vector3(radius * 2f, radius * 2f, 1f);
        float mult = expandOnExplode ? startScaleMultiplier : 1f;
        transform.localScale = baseScale * mult;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = startColor;
            spriteRenderer.enabled = true;
        }

        spawnTime = Time.time;
        Destroy(gameObject, lifeTime);

        if (explodeOnSpawn)
        {
            Explode();
        }
    }

    void Update()
    {
        float t = (Time.time - spawnTime) / Mathf.Max(0.0001f, lifeTime);
        t = Mathf.Clamp01(t);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(startColor, endColor, t);
        }

        if (expandOnExplode)
        {
            float mult = Mathf.Lerp(startScaleMultiplier, endScaleMultiplier, t);
            transform.localScale = baseScale * mult;
        }
    }

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy == null) continue;

            enemy.TakeDamage(damage);

            if (applySlow)
            {
                EnemyFollow follow = hit.GetComponent<EnemyFollow>();
                if (follow != null)
                {
                    follow.ApplySlow(slowDuration, slowFactor);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;
        if (other.GetComponent<EnemyHealth>() == null) return;
        Explode();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
