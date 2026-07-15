using System.Collections;
using UnityEngine;

namespace Assets.script
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BossCodeOverload : MonoBehaviour
    {
        [Header("Cooldown")]
        public float cooldown = 12f;

        [Header("Giai đoạn 1: Telegraph (đứng khựng lại, báo hiệu sắp nhảy)")]
        [Tooltip("Thời gian đứng yên trước khi nhảy - đây là khoảng Player có thể quan sát và né")]
        public float telegraphTime = 0.6f;

        [Header("Giai đoạn 2: Nhảy vọt tới gần Player")]
        [Tooltip("Thời gian bay từ lúc bắt đầu nhảy tới lúc hạ xuống - càng ngắn càng khó né")]
        public float jumpDuration = 0.35f;
        [Tooltip("Khoảng cách lệch so với vị trí Player lúc chốt mục tiêu, tránh Boss rơi đè thẳng lên người")]
        public float landOffset = 1f;
        [Tooltip("Độ cao vòng cung khi bay - giúp phân biệt với BossDash (bay thẳng), 0 = bay thẳng như cũ")]
        public float arcHeight = 1.5f;

        [Header("Giai đoạn 3: Hạ cánh - Windup trước khi nổ")]
        [Tooltip("Khoảng dừng SAU khi chạm đất, TRƯỚC khi nổ thật - đây là lúc Player còn kịp chạy ra khỏi vòng nổ. Đây là điểm khác biệt chính so với BossDash.")]
        public float explodeDelay = 0.35f;
        [Tooltip("Thời gian giữ hiệu ứng nổ (Boss đứng khựng lại sau cú nổ) trước khi quay lại di chuyển bình thường")]
        public float explodeHoldTime = 0.6f;
        public float explosionRadius = 3f;
        public int overloadDamage = 6;
        [Tooltip("Layer của Player, để OverlapCircle chỉ bắt trúng Player")]
        public LayerMask playerLayer;

        [Header("Giai đoạn 4: Vùng ảnh hưởng còn sót lại sau khi nổ (loang lổ, độc lập với Boss)")]
        [Tooltip("Vùng này tồn tại độc lập - Boss có thể di chuyển đi chỗ khác, vùng vẫn ở lại đúng thời gian này rồi mới biến mất")]
        public float lingerDuration = 6.5f;
        [Tooltip("Cứ mỗi khoảng này, nếu Player còn đứng trong vùng sẽ bị trừ máu 1 lần")]
        public float lingerTickInterval = 1f;
        public int lingerDamagePerTick = 2;
        [Tooltip("Bán kính vùng loang lổ. Để <= 0 sẽ tự dùng bằng explosionRadius")]
        public float lingerRadius = -1f;
        public Color lingerColor = new Color(0.6f, 0.3f, 1f, 0.35f);

        [Header("Warning Circle (tự vẽ bằng LineRenderer, không cần prefab)")]
        public Color warningColor = new Color(0.6f, 0.3f, 1f, 0.8f); // tím, khớp Code Overload
        public float warningLineWidth = 0.08f;
        [Tooltip("Số điểm vẽ vòng tròn, càng cao càng mượt, 32-40 là đủ đẹp")]
        public int warningSegments = 36;
        [Tooltip("Sorting order của vòng tròn cảnh báo / vùng loang lổ, tăng lên nếu bị tilemap/background che mất")]
        public int warningSortingOrder = 10;
        [Tooltip("Nếu project dùng Sorting Layer riêng cho Boss/Player, điền tên layer vào đây, để trống nếu không dùng")]
        public string warningSortingLayerName = "";

        [Header("Animator")]
        [Tooltip("Kéo Animator của Boss vào đây (đã có sẵn Trigger 'Attack4' trong Controller)")]
        public Animator animator;
        [Tooltip("Tên Trigger trong Animator Controller, khớp với param bạn đã tạo")]
        public string attackTrigger = "Attack4";
        [Tooltip("Tên Trigger cho giai đoạn telegraph (nếu Animator có state Telegraph riêng, để trống nếu không dùng)")]
        public string telegraphTrigger = "Telegraph";
        [Tooltip("Tên Trigger cho lúc chạm đất (nếu Animator có state riêng, để trống nếu không dùng)")]
        public string landTrigger = "";

        [Header("Visual Feedback dự phòng (chỉ dùng nếu KHÔNG gán Animator)")]
        public SpriteRenderer sr;
        public Color chargeColor = new Color(0.6f, 0.3f, 1f); // tím, khớp hiệu ứng Code Overload

        public bool IsCasting { get; private set; } = false;

        private Color originalColor;
        private Rigidbody2D rb;
        private Transform player;
        private float cooldownTimer;
        private bool isBusy = false;
        private BossFollow follow;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            follow = GetComponent<BossFollow>();
            if (animator == null) animator = GetComponent<Animator>();
            if (sr == null) sr = GetComponent<SpriteRenderer>();
            if (sr != null) originalColor = sr.color;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;

            cooldownTimer = cooldown;
        }

        void Update()
        {
            if (isBusy || player == null) return;

            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                cooldownTimer = cooldown;
                StartCoroutine(OverloadRoutine());
            }
        }

        IEnumerator OverloadRoutine()
        {
            isBusy = true;
            IsCasting = true;

            // ===== GIAI ĐOẠN 1: TELEGRAPH - đứng khựng lại, báo hiệu sắp nhảy =====
            if (follow != null) follow.SetMovementEnabled(false);

            if (animator != null && !string.IsNullOrEmpty(telegraphTrigger))
            {
                animator.SetTrigger(telegraphTrigger);
            }
            else if (sr != null)
            {
                sr.color = chargeColor;
            }

            yield return new WaitForSeconds(telegraphTime);

            if (sr != null) sr.color = originalColor;

            // ===== GIAI ĐOẠN 2: NHẢY VỌT (theo vòng cung) - chốt vị trí Player NGAY LÚC NÀY =====
            // Chốt 1 lần duy nhất tại đây (không đuổi theo trong lúc bay)
            // -> Player vẫn còn thời gian né bằng cách di chuyển ra khỏi điểm này trước khi Boss rơi xuống
            Vector2 startPos = rb.position;
            Vector2 targetPos = player != null ? (Vector2)player.position : startPos;

            // Lệch nhẹ ra khỏi vị trí Player để không rơi đè khít lên người
            Vector2 dirFromStart = (targetPos - startPos).sqrMagnitude > 0.01f
                ? (targetPos - startPos).normalized
                : Vector2.zero;
            Vector2 landPos = targetPos - dirFromStart * landOffset;

            if (animator != null)
                animator.SetTrigger(attackTrigger);

            float t = 0f;
            while (t < jumpDuration)
            {
                float progress = t / jumpDuration;
                Vector2 flatPos = Vector2.Lerp(startPos, landPos, progress);
                // Vòng cung: nhô cao ở giữa quãng đường, giúp phân biệt rõ với BossDash (bay thẳng tuyến tính)
                float arcOffset = Mathf.Sin(progress * Mathf.PI) * arcHeight;
                rb.MovePosition(flatPos + Vector2.up * arcOffset);

                t += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            rb.MovePosition(landPos);

            // ===== GIAI ĐOẠN 3a: HẠ CÁNH - hiện vòng tròn cảnh báo, CHƯA NỔ NGAY =====
            // Đây là nhịp riêng của CodeOverload: Player phải đọc thêm vùng nổ sắp xảy ra,
            // khác với BossDash chỉ có 1 nhịp né duy nhất lúc đang lao.
            if (animator != null && !string.IsNullOrEmpty(landTrigger))
                animator.SetTrigger(landTrigger);

            SpawnWarningCircle(landPos, explosionRadius, explodeDelay);

            yield return new WaitForSeconds(explodeDelay);

            // ===== GIAI ĐOẠN 3b: NỔ AOE THẬT TẠI ĐIỂM RƠI =====
            Explode(landPos);

            // ===== GIAI ĐOẠN 4: VÙNG LOANG LỔ CÒN SÓT LẠI =====
            // Spawn ra 1 GameObject độc lập, tự tồn tại lingerDuration giây rồi tự huỷ,
            // không phụ thuộc vào coroutine của Boss (Boss quay lại di chuyển bình thường ngay sau explodeHoldTime).
            SpawnLingerZone(landPos);

            yield return new WaitForSeconds(explodeHoldTime);

            if (follow != null) follow.SetMovementEnabled(true);

            IsCasting = false;
            isBusy = false;
        }

        void Explode(Vector2 center)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, explosionRadius, playerLayer);
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Player")) continue;

                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(overloadDamage);
                    Debug.Log("Boss CODE OVERLOAD hạ cánh trúng Player, mất " + overloadDamage + " máu!");
                }
            }
        }

        // Tự vẽ vòng tròn cảnh báo bằng LineRenderer, không cần prefab/sprite ngoài.
        // Tồn tại đúng "duration" giây rồi tự huỷ (khớp với explodeDelay để biến mất đúng lúc nổ).
        GameObject SpawnWarningCircle(Vector2 center, float radius, float duration)
        {
            GameObject warningObj = new GameObject("ExplosionWarning");
            warningObj.transform.position = center;

            LineRenderer lr = warningObj.AddComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.loop = true;
            lr.positionCount = warningSegments;
            lr.widthMultiplier = warningLineWidth;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = warningColor;
            lr.endColor = warningColor;
            lr.sortingOrder = warningSortingOrder;

            if (!string.IsNullOrEmpty(warningSortingLayerName))
                lr.sortingLayerName = warningSortingLayerName;

            for (int i = 0; i < warningSegments; i++)
            {
                float angle = i * Mathf.PI * 2f / warningSegments;
                lr.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f));
            }

            Destroy(warningObj, duration);
            return warningObj;
        }

        // Spawn vùng ảnh hưởng còn sót lại sau vụ nổ (script riêng ExplosionLingerZone.cs).
        // Vùng này gây damage theo tick cho Player đứng trong đó, tồn tại độc lập với Boss.
        void SpawnLingerZone(Vector2 center)
        {
            float radius = lingerRadius > 0f ? lingerRadius : explosionRadius;

            GameObject zoneObj = new GameObject("ExplosionLingerZone");
            ExplosionLingerZone zone = zoneObj.AddComponent<ExplosionLingerZone>();
            zone.Setup(
                center,
                radius,
                lingerDuration,
                lingerTickInterval,
                lingerDamagePerTick,
                playerLayer,
                lingerColor,
                warningLineWidth,
                warningSegments,
                warningSortingOrder,
                warningSortingLayerName
            );
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.6f, 0.3f, 1f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}