using System.Collections;
using UnityEngine;

namespace Assets.script
{
    // Vùng "loang lổ" còn sót lại sau khi CodeOverload nổ.
    // Tự spawn ra 1 GameObject riêng, KHÔNG phụ thuộc vào Boss (Boss có thể đi chỗ khác,
    // vùng này vẫn tồn tại đúng thời gian rồi tự huỷ).
    // Player đứng trong vùng sẽ bị trừ máu đều đặn theo tickInterval.
    public class ExplosionLingerZone : MonoBehaviour
    {
        private float radius;
        private float tickInterval;
        private int damagePerTick;
        private LayerMask playerLayer;

        public void Setup(Vector2 center, float radius, float duration, float tickInterval,
            int damagePerTick, LayerMask playerLayer, Color color, float lineWidth, int segments,
            int sortingOrder, string sortingLayerName)
        {
            transform.position = center;
            this.radius = radius;
            this.tickInterval = tickInterval;
            this.damagePerTick = damagePerTick;
            this.playerLayer = playerLayer;

            DrawZoneVisual(color, segments, sortingOrder, sortingLayerName);

            StartCoroutine(TickDamage());
            Destroy(gameObject, duration);
        }

        // Vẽ vòng tròn TÔ ĐẶC (fill) toàn bộ bằng màu color, thay vì chỉ có viền.
        // Dùng Mesh dạng "quạt tam giác" (triangle fan) từ tâm ra viền, nhẹ hơn nhiều so với sprite runtime.
        void DrawZoneVisual(Color color, int segments, int sortingOrder, string sortingLayerName)
        {
            MeshFilter mf = gameObject.AddComponent<MeshFilter>();
            MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();

            Mesh mesh = new Mesh();

            // Tâm + các điểm trên viền -> tổng segments + 1 đỉnh
            Vector3[] vertices = new Vector3[segments + 1];
            Color[] colors = new Color[segments + 1];
            int[] triangles = new int[segments * 3];

            vertices[0] = Vector3.zero; // tâm
            colors[0] = color;

            for (int i = 0; i < segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                colors[i + 1] = color;
            }

            for (int i = 0; i < segments; i++)
            {
                int next = (i + 1) % segments;
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = next + 1;
            }

            mesh.vertices = vertices;
            mesh.colors = colors;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();

            mf.mesh = mesh;

            Material mat = new Material(Shader.Find("Sprites/Default"));
            mr.material = mat;
            mr.sortingOrder = sortingOrder;

            if (!string.IsNullOrEmpty(sortingLayerName))
                mr.sortingLayerName = sortingLayerName;
        }

        IEnumerator TickDamage()
        {
            Debug.Log("[LingerZone] Đã spawn tại " + transform.position + ", radius=" + radius +
                ", playerLayer=" + playerLayer.value + " (0 = CHƯA GÁN LAYER trong Inspector!)");

            // Gây damage ngay lượt đầu tiên (vừa bước vào vùng là dính luôn),
            // sau đó lặp lại mỗi tickInterval giây cho tới khi vùng biến mất.
            while (true)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, playerLayer);

                // DEBUG: log ra số lượng collider bắt được mỗi tick, kể cả khi = 0,
                // để biết OverlapCircle có đang quét trúng gì không.
                Debug.Log("[LingerZone] Tick - tìm thấy " + hits.Length + " collider trong vùng (layer mask=" + playerLayer.value + ")");

                foreach (var hit in hits)
                {
                    Debug.Log("[LingerZone] -> Va chạm: " + hit.gameObject.name + " (tag=" + hit.tag + ")");

                    if (!hit.CompareTag("Player"))
                    {
                        Debug.Log("[LingerZone] -> Bỏ qua vì tag không phải 'Player'");
                        continue;
                    }

                    PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                    if (playerHealth == null)
                    {
                        Debug.LogWarning("[LingerZone] -> Object có tag Player nhưng KHÔNG có component PlayerHealth!");
                        continue;
                    }

                    playerHealth.TakeDamage(damagePerTick);
                    Debug.Log("Player đứng trong vùng CODE OVERLOAD còn sót lại, mất " + damagePerTick + " máu!");
                }

                yield return new WaitForSeconds(tickInterval);
            }
        }

        // Vẽ vùng ảnh hưởng trong Scene View để dễ canh chỉnh
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.6f, 0.3f, 1f, 0.2f);
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}