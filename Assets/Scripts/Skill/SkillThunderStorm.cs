using System.Collections.Generic;
using UnityEngine;

public class SkillThunderStorm : MonoBehaviour
{
    [Header("Thunder Settings")]
    [Tooltip("Số lượng sét đánh xuống")]
    public int thunderCount = 10;

    [Tooltip("Thời gian delay trước khi sét đánh (1 giây)")]
    public float strikeDelay = 1f;

    [Tooltip("Damage mỗi đợt sét")]
    public int damagePerStrike = 50;

    [Header("Target Settings")]
    [Tooltip("Bán kính tìm quái gần nhất")]
    public float searchRadius = 999f;

    [Tooltip("Bán kính mỗi vùng sét (cỡ 2 người chơi)")]
    public float strikeRadius = 2f;

    [Tooltip("Layer của enemy")]
    public LayerMask enemyMask = ~0;

    [Header("Visual Prefabs")]
    [Tooltip("Prefab ô đỏ warning (SpriteRenderer đơn giản)")]
    public GameObject warningPrefab;

    [Tooltip("Prefab sấm sét (của bạn)")]
    public GameObject lightningPrefab;

    [Header("Warning Zone Settings")]
    public Color warningColor = new Color(1f, 0f, 0f, 0.4f);

    private List<Vector3> strikePositions = new List<Vector3>();
    private List<GameObject> warningZones = new List<GameObject>();

    void OnEnable()
    {
        FindTargetsAndSpawnWarnings();
        Invoke(nameof(StrikeThunder), strikeDelay);
        Destroy(gameObject, strikeDelay + 2f);
    }

    void FindTargetsAndSpawnWarnings()
    {
        // Tìm tất cả enemy trong bán kính
        Collider2D[] allEnemies = Physics2D.OverlapCircleAll(transform.position, searchRadius, enemyMask);

        // Tạo list enemy với khoảng cách
        List<EnemyTarget> enemies = new List<EnemyTarget>();
        foreach (Collider2D col in allEnemies)
        {
            EnemyHealth health = col.GetComponent<EnemyHealth>();
            if (health != null)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                enemies.Add(new EnemyTarget
                {
                    collider = col,
                    health = health,
                    distance = distance
                });
            }
        }

        // Sort theo khoảng cách gần nhất
        enemies.Sort((a, b) => a.distance.CompareTo(b.distance));

        // Chọn target cho thunderCount lần
        for (int i = 0; i < thunderCount; i++)
        {
            Vector3 targetPos;

            if (enemies.Count == 0)
            {
                // Không có quái, skip
                break;
            }
            else if (i < enemies.Count)
            {
                // Có đủ quái, target quái thứ i
                targetPos = enemies[i].collider.transform.position;
            }
            else
            {
                // Ít hơn thunderCount quái, target vào thằng máu nhiều nhất
                EnemyTarget maxHealthEnemy = enemies[0];
                foreach (var enemy in enemies)
                {
                    if (enemy.health.CurrentHealth > maxHealthEnemy.health.CurrentHealth)
                    {
                        maxHealthEnemy = enemy;
                    }
                }
                targetPos = maxHealthEnemy.collider.transform.position;
            }

            strikePositions.Add(targetPos);
            SpawnWarningZone(targetPos);
        }
    }

    private class EnemyTarget
    {
        public Collider2D collider;
        public EnemyHealth health;
        public float distance;
    }

    void SpawnWarningZone(Vector3 position)
    {
        GameObject warning;

        if (warningPrefab != null)
        {
            warning = Instantiate(warningPrefab, position, Quaternion.identity, transform);
        }
        else
        {
            warning = new GameObject("ThunderWarning");
            warning.transform.position = position;
            warning.transform.SetParent(transform);

            SpriteRenderer sr = warning.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite(128);
            sr.color = warningColor;
            sr.sortingOrder = -1;
        }

        warning.transform.localScale = Vector3.one * strikeRadius * 2f;
        warningZones.Add(warning);
    }


    void StrikeThunder()
    {
        // Xóa các warning zone
        foreach (GameObject warning in warningZones)
        {
            if (warning != null)
            {
                Destroy(warning);
            }
        }

        // Spawn lightning và gây damage tại các vị trí đã lưu
        foreach (Vector3 pos in strikePositions)
        {
            // Spawn lightning visual
            if (lightningPrefab != null)
            {
                GameObject lightning = Instantiate(lightningPrefab, pos, Quaternion.identity);
                Destroy(lightning, 1f);
            }

            // Gây damage cho enemy trong vùng
            Collider2D[] hits = Physics2D.OverlapCircleAll(pos, strikeRadius, enemyMask);
            foreach (Collider2D hit in hits)
            {
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damagePerStrike);
                }
            }
        }
    }

    Sprite CreateCircleSprite(int resolution)
    {
        Texture2D texture = new Texture2D(resolution, resolution);
        Color[] pixels = new Color[resolution * resolution];

        float center = resolution / 2f;
        float radius = center;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);

                // Vẽ vòng tròn với anti-aliasing
                if (distance < radius - 1)
                {
                    pixels[y * resolution + x] = Color.white;
                }
                else if (distance < radius)
                {
                    float alpha = radius - distance;
                    pixels[y * resolution + x] = new Color(1, 1, 1, alpha);
                }
                else
                {
                    pixels[y * resolution + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, resolution, resolution), new Vector2(0.5f, 0.5f), 100f);
    }
}
