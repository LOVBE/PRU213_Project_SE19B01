using System.Collections.Generic;
using UnityEngine;

public class SkillFireBurst : MonoBehaviour
{
    [Header("Duration")]
    [Tooltip("Thời gian tồn tại của kỹ năng")]
    public float duration = 3f;

    [Header("Area Settings")]
    [Tooltip("Bán kính vùng lửa")]
    public float radius = 4f;

    [Tooltip("Mật độ đốm lửa (số lượng trên 1 đơn vị diện tích)")]
    public float fireDensity = 3f;

    [Header("Fire Prefab")]
    [Tooltip("Prefab sprite đơn giản cho đốm lửa (chỉ cần SpriteRenderer)")]
    public GameObject firePrefab;

    [Tooltip("Kích thước của mỗi đốm lửa")]
    public float fireSize = 0.4f;

    [Header("Flicker Settings")]
    [Tooltip("Tỷ lệ đốm lửa hiện (0.66 = 2/3 hiện, 1/3 tắt)")]
    [Range(0.3f, 0.9f)]
    public float visibleRatio = 0.66f;

    [Tooltip("Tốc độ thay đổi nhấp nháy (lần/giây)")]
    public float flickerRate = 10f;

    [Header("Colors")]
    public Color fireColorBright = new Color(1f, 0.8f, 0f, 1f);
    public Color fireColorDim = new Color(1f, 0.3f, 0f, 0f);

    [Header("Damage")]
    [Tooltip("Tổng damage trong suốt 3 giây")]
    public int totalDamage = 30;

    [Tooltip("Số lần gây damage")]
    public int damageTickCount = 6;

    [Tooltip("Layer của enemy")]
    public LayerMask enemyMask = ~0;

    private List<GameObject> fires = new List<GameObject>();
    private List<SpriteRenderer> fireRenderers = new List<SpriteRenderer>();
    private float startTime;
    private float nextDamageTick;
    private float nextFlickerUpdate;
    private float damageInterval;
    private float flickerInterval;
    private int damagePerTick;

    void OnEnable()
    {
        startTime = Time.time;
        damageInterval = duration / damageTickCount;
        flickerInterval = 1f / flickerRate;
        nextDamageTick = startTime + damageInterval;
        nextFlickerUpdate = startTime;
        damagePerTick = Mathf.Max(1, totalDamage / damageTickCount);

        SpawnFireArea();
        UpdateFlicker();
    }

    void SpawnFireArea()
    {
        if (firePrefab == null)
        {
            Debug.LogWarning("[SkillFireBurst] Fire prefab chưa được gán!");
            return;
        }

        // Tính số lượng đốm lửa cần spawn dựa trên diện tích và mật độ
        float area = Mathf.PI * radius * radius;
        int fireCount = Mathf.RoundToInt(area * fireDensity);

        // Spawn các đốm lửa ngẫu nhiên trong vùng tròn để fill kín
        for (int i = 0; i < fireCount; i++)
        {
            // Random position trong vòng tròn (uniform distribution)
            Vector2 randomPoint = Random.insideUnitCircle * radius;
            Vector3 pos = transform.position + new Vector3(randomPoint.x, randomPoint.y, 0f);

            GameObject fire = Instantiate(firePrefab, pos, Quaternion.identity, transform);
            fire.transform.localScale = Vector3.one * fireSize;

            // Lấy hoặc thêm SpriteRenderer
            SpriteRenderer sr = fire.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = fire.AddComponent<SpriteRenderer>();
            }

            // Tắt các component không cần thiết
            SkillAOE aoe = fire.GetComponent<SkillAOE>();
            if (aoe != null)
            {
                Destroy(aoe);
            }

            fires.Add(fire);
            fireRenderers.Add(sr);
        }
    }

    void Update()
    {
        float elapsed = Time.time - startTime;

        // Kết thúc sau duration
        if (elapsed >= duration)
        {
            Destroy(gameObject);
            return;
        }

        // Cập nhật hiệu ứng nhấp nháy
        if (Time.time >= nextFlickerUpdate)
        {
            UpdateFlicker();
            nextFlickerUpdate = Time.time + flickerInterval;
        }

        // Gây damage theo interval
        if (Time.time >= nextDamageTick)
        {
            ApplyDamage();
            nextDamageTick = Time.time + damageInterval;
        }
    }

    void UpdateFlicker()
    {
        // Tính số lượng đốm lửa sẽ hiện (2/3)
        int visibleCount = Mathf.RoundToInt(fireRenderers.Count * visibleRatio);

        // Tạo danh sách index và shuffle
        List<int> indices = new List<int>();
        for (int i = 0; i < fireRenderers.Count; i++)
        {
            indices.Add(i);
        }

        // Shuffle để random
        for (int i = 0; i < indices.Count; i++)
        {
            int randomIndex = Random.Range(i, indices.Count);
            int temp = indices[i];
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        // Set hiển thị: visibleCount đầu tiên sẽ hiện, còn lại tắt
        for (int i = 0; i < fireRenderers.Count; i++)
        {
            if (fireRenderers[i] == null) continue;

            if (i < visibleCount)
            {
                // Hiện với màu sáng
                fireRenderers[i].color = fireColorBright;
            }
            else
            {
                // Tắt hoặc mờ đi
                fireRenderers[i].color = fireColorDim;
            }
        }
    }

    void ApplyDamage()
    {
        // Gây damage cho tất cả enemy trong vùng tròn
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damagePerTick);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, radius);

        Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
