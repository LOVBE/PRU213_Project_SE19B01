using System.Collections.Generic;
using UnityEngine;

public class SkillIceNova : MonoBehaviour
{
    [Header("Ice Nova Settings")]
    [Tooltip("Thời gian làm chậm (2 giây)")]
    public float slowDuration = 2f;

    [Tooltip("Tỷ lệ làm chậm (0.01 = chậm 99%)")]
    [Range(0.01f, 0.5f)]
    public float slowFactor = 0.01f;

    [Header("Area Settings")]
    [Tooltip("Bán kính ảnh hưởng (để rất lớn = toàn map)")]
    public float radius = 999f;

    [Tooltip("Layer của enemy")]
    public LayerMask enemyMask = ~0;

    [Header("Visual Settings")]
    [Tooltip("Màu băng giá áp lên enemy")]
    public Color iceColor = new Color(0.5f, 0.8f, 1f, 1f); // Xanh băng

    [Tooltip("Cường độ áp màu băng (0-1)")]
    [Range(0f, 1f)]
    public float iceColorStrength = 0.7f;

    private class FrozenEnemy
    {
        public EnemyFollow enemyFollow;
        public SpriteRenderer spriteRenderer;
        public Color originalColor;
    }

    private List<FrozenEnemy> frozenEnemies = new List<FrozenEnemy>();
    private float startTime;

    void OnEnable()
    {
        startTime = Time.time;
        ApplyIceEffect();
    }

    void ApplyIceEffect()
    {
        // Tìm tất cả enemy trong bán kính (radius lớn = toàn map)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyMask);

        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;

            // Lấy EnemyFollow để apply slow
            EnemyFollow enemyFollow = hit.GetComponent<EnemyFollow>();
            if (enemyFollow == null) continue;

            // Lấy SpriteRenderer để đổi màu
            SpriteRenderer sr = hit.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            // Lưu thông tin enemy
            FrozenEnemy frozen = new FrozenEnemy
            {
                enemyFollow = enemyFollow,
                spriteRenderer = sr,
                originalColor = sr.color
            };
            frozenEnemies.Add(frozen);

            // Apply slow effect
            enemyFollow.ApplySlow(slowDuration, slowFactor);

            // Đổi màu sang xanh băng
            Color newColor = Color.Lerp(frozen.originalColor, iceColor, iceColorStrength);
            sr.color = newColor;
        }
    }

    void Update()
    {
        float elapsed = Time.time - startTime;

        // Sau slowDuration giây, restore và destroy
        if (elapsed >= slowDuration)
        {
            RestoreEnemies();
            Destroy(gameObject);
        }
    }

    void RestoreEnemies()
    {
        // Khôi phục màu gốc cho tất cả enemy bị đóng băng
        foreach (FrozenEnemy frozen in frozenEnemies)
        {
            if (frozen.spriteRenderer != null)
            {
                frozen.spriteRenderer.color = frozen.originalColor;
            }
        }

        frozenEnemies.Clear();
    }

    void OnDestroy()
    {
        // Đảm bảo restore nếu object bị destroy bất ngờ
        RestoreEnemies();
    }
}
