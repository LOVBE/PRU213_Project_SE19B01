using System.Collections;
using UnityEngine;
using Assets.script;

[RequireComponent(typeof(Rigidbody2D))]
public class BossDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashCooldown = 5f;      // Khoảng cách thời gian giữa 2 lần dash
    public float telegraphTime = 0.5f;   // Thời gian đứng yên báo trước khi lao (để Player kịp né)
    public float dashSpeed = 14f;        // Tốc độ lúc lao, nên nhanh hơn hẳn moveSpeed thường (vd 2)

    [Header("Dash Duration (tự động tính theo khoảng cách)")]
    [Tooltip("Giới hạn dưới: tránh dash quá ngắn/giật cục lúc đứng sát Player")]
    public float minDashDuration = 0.15f;
    [Tooltip("Giới hạn trên: tránh lao quá lâu nếu khoảng cách quá xa")]
    public float maxDashDuration = 1f;

    [Header("Dash Damage")]
    public int dashDamage = 4;           // Damage gây ra khi va chạm Player LÚC ĐANG DASH (cao hơn damage thường)

    // Cho script khác (BossFollow) kiểm tra xem Boss có đang trong lúc lao hay không
    public bool IsDashing { get; private set; } = false;

    [Header("Visual Feedback (tuỳ chọn)")]
    public SpriteRenderer sr;            // Để trống sẽ tự tìm trên chính object này
    public Color telegraphColor = new Color(1f, 0.4f, 0.4f); // màu báo hiệu sắp lao

    private Rigidbody2D rb;
    private Transform player;
    private BossFollow follow;
    private Color originalColor;

    private Vector2 dashDirection;
    private float cooldownTimer;
    private bool isBusy = false; // đang trong lúc telegraph hoặc đang dash

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        follow = GetComponent<BossFollow>(); // Lấy đúng BossFollow để gọi SetMovementEnabled

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;

        cooldownTimer = dashCooldown; // tránh dash ngay lúc vừa xuất hiện
    }

    void Update()
    {
        if (player == null || isBusy) return;

        // Cooldown luôn đếm liên tục, KHÔNG phụ thuộc khoảng cách tới Player nữa.
        // Nếu Player đứng xa, Dash vẫn kích hoạt và rút ngắn khoảng cách (giới hạn bởi maxDashDuration),
        // tránh tình trạng Player chạy xa khiến Dash không bao giờ kích hoạt được.
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            cooldownTimer = dashCooldown;
            StartCoroutine(DashRoutine());
        }
    }

    IEnumerator DashRoutine()
    {
        isBusy = true;

        // 1. Telegraph: đứng yên tại chỗ, đổi màu báo hiệu sắp lao
        if (follow != null) follow.SetMovementEnabled(false);
        if (sr != null) sr.color = telegraphColor;

        yield return new WaitForSeconds(telegraphTime);

        if (sr != null) sr.color = originalColor;

        // 2. Chốt hướng + tính quãng đường tới ĐÚNG vị trí Player NGAY thời điểm này
        //    (Player vẫn di chuyển bình thường được trong và sau lúc này, boss không "bám" theo nữa)
        Vector2 startPos = rb.position;
        Vector2 targetPos = player != null ? (Vector2)player.position : startPos;
        float distance = Vector2.Distance(startPos, targetPos);
        dashDirection = distance > 0.01f ? (targetPos - startPos).normalized : Vector2.zero;

        // Thời gian lao = quãng đường / tốc độ, giới hạn trong khoảng an toàn (min/max)
        float computedDuration = Mathf.Clamp(distance / dashSpeed, minDashDuration, maxDashDuration);

        // 3. Thực hiện lao trong computedDuration giây
        IsDashing = true;
        float t = 0f;
        while (t < computedDuration)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        IsDashing = false;

        // 4. Kết thúc dash, trả lại quyền bám đuổi bình thường cho EnemyFollow
        if (follow != null) follow.SetMovementEnabled(true);

        isBusy = false;
    }
}