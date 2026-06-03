using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("Boss Setup")]
    public GameObject bossPrefab;    // Kéo Prefab con Boss vào đây
    public Transform spawnPoint;     // Kéo Object vị trí đẻ Boss vào đây

    [Header("Spawn Settings")]
    public float timeToSpawn = 60f;  // Thời gian đếm ngược (để 60 giây theo ý ông)

    private float timer = 0f;
    private bool hasSpawned = false; // Biến cờ để đánh dấu xem đã đẻ Boss chưa

    void Update()
    {
        // 1. Nếu Boss đã được đẻ ra rồi thì dừng luôn, không đếm giờ hay làm gì thêm nữa (Giới hạn đúng 1 con)
        if (hasSpawned) return;

        // 2. Bắt đầu đếm thời gian từ lúc vào game
        timer += Time.deltaTime;

        // 3. Khi đồng hồ điểm đúng 60 giây (hoặc hơn)
        if (timer >= timeToSpawn)
        {
            SpawnBoss();

            // Khóa chốt lại để hàm Update không bao giờ đẻ thêm con thứ 2
            hasSpawned = true;
        }
    }

    void SpawnBoss()
    {
        // Kiểm tra an toàn xem có lỡ quên kéo thả trong Inspector không
        if (bossPrefab != null && spawnPoint != null)
        {
            Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log("Đã đủ 60s. Boss Bã Mía xuất hiện!!"); // In ra Console để dễ theo dõi
        }
        else
        {
            Debug.LogWarning("Chưa kéo Boss Prefab hoặc Spawn Point vào BossSpawner kìa ông ơi!");
        }
    }
}