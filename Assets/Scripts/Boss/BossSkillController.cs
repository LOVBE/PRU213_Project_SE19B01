using System.Collections;
using Assets.script;
using UnityEngine;

/// <summary>
/// Điều khiển chuỗi skill: 
/// Idle (Boss đuổi theo Player bình thường qua BossFollow) 
/// -> sau idleDuration giây, biến thành cơn lốc (animation) 
/// -> dash liên tục dashRepeatCount lần (dùng lại BossDash có sẵn) 
/// -> quay lại Idle -> lặp vô hạn.
/// 
/// Không viết lại logic dash, chỉ điều phối THỜI ĐIỂM gọi BossDash.PerformDash().
/// </summary>
[RequireComponent(typeof(BossDash))]
public class BossSkillController : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("Thời gian Boss đuổi theo Player bình thường trước khi biến lốc")]
    public float idleDuration = 3f;

    [Tooltip("Thời gian giữ animation biến lốc trước khi bắt đầu dash")]
    public float whirlwindDuration = 1f;

    [Tooltip("Số lần dash liên tiếp sau khi biến lốc")]
    public int dashRepeatCount = 3;

    [Tooltip("Khoảng nghỉ giữa mỗi lần dash (0 = dash liên tục không nghỉ)")]
    public float intervalBetweenDashes = 0.1f;

    [Header("Animator Triggers (phải khớp tên Parameter trong Animator Controller)")]
    public string whirlwindTrigger = "Whirlwind";
    public string dashTrigger = "Dash";
    public string idleTrigger = "Idle";

    [Header("References")]
    public Animator animator;

    private BossDash bossDash;
    private BossFollow bossFollow;

    void Awake()
    {
        bossDash = GetComponent<BossDash>();
        bossFollow = GetComponent<BossFollow>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Tắt cơ chế tự dash theo timer riêng của BossDash,
        // để BossSkillController toàn quyền quyết định KHI NÀO dash.
        if (bossDash != null) bossDash.autoTrigger = false;

        StartCoroutine(SkillLoop());
    }

    IEnumerator SkillLoop()
    {
        while (true)
        {
            yield return StartCoroutine(IdlePhase());
            yield return StartCoroutine(WhirlwindPhase());
            yield return StartCoroutine(DashPhase());
        }
    }

    IEnumerator IdlePhase()
    {
        // Không cần tắt/bật gì cả - BossFollow đang tự chạy đuổi theo Player bình thường.
        if (animator != null && !string.IsNullOrEmpty(idleTrigger))
            animator.SetTrigger(idleTrigger);

        yield return new WaitForSeconds(idleDuration);
    }

    IEnumerator WhirlwindPhase()
    {
        // Dừng việc đuổi theo Player lại trong lúc biến hình, cho animation rõ ràng
        if (bossFollow != null) bossFollow.SetMovementEnabled(false);

        if (animator != null && !string.IsNullOrEmpty(whirlwindTrigger))
            animator.SetTrigger(whirlwindTrigger);

        yield return new WaitForSeconds(whirlwindDuration);
    }

    IEnumerator DashPhase()
    {
        for (int i = 0; i < dashRepeatCount; i++)
        {
            if (animator != null && !string.IsNullOrEmpty(dashTrigger))
                animator.SetTrigger(dashTrigger);

            // Gọi lại đúng logic dash có sẵn trong BossDash (telegraph + lao + damage),
            // không viết lại gì cả.
            yield return StartCoroutine(bossDash.PerformDash());

            if (i < dashRepeatCount - 1 && intervalBetweenDashes > 0f)
                yield return new WaitForSeconds(intervalBetweenDashes);
        }

        // Sau lần dash cuối, BossDash.PerformDash() đã tự bật lại follow.SetMovementEnabled(true)
        // -> Boss quay lại đuổi theo Player bình thường, đúng như IdlePhase mong muốn.
    }
}