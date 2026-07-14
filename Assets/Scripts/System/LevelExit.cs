using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelExitTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lockedHintUI;
    [SerializeField] private Image fadePanel;
    [SerializeField] private TMP_Text youWinText;

    [Header("Door Glow")]
    [SerializeField] private SpriteRenderer doorGlow;
    [SerializeField] private float glowSpeed = 2f;
    [SerializeField] private float minGlowAlpha = 0.3f;
    [SerializeField] private float maxGlowAlpha = 1f;

    [Header("Scene Settings")]
    [Tooltip("Tên scene sẽ chuyển tới sau khi hoàn thành level")]
    [SerializeField] private string nextSceneName;

    [Tooltip("Nếu bật, level không cần boss chết vẫn có thể kết thúc")]
    [SerializeField] private bool requireBossDefeated = true;

    [Tooltip("Thời gian fade màn hình")]
    [SerializeField] private float fadeDuration = 1f;

    [Tooltip("Thời gian hiện chữ You Win")]
    [SerializeField] private float winTextDuration = 1f;

    [Tooltip("Thời gian chờ trước khi đổi scene")]
    [SerializeField] private float delayBeforeLoad = 2f;

    private bool bossIsDead;
    private bool isLoading;
    private bool glowDoor;

    private void Awake()
    {
        PrepareUI();

        if (!requireBossDefeated)
        {
            bossIsDead = true;
            glowDoor = true;
        }
    }

    private void OnEnable()
    {
        EnemyHealth.OnBossDied += HandleBossDied;
    }

    private void OnDisable()
    {
        EnemyHealth.OnBossDied -= HandleBossDied;
    }

    private void Update()
    {
        UpdateDoorGlow();
    }

    private void PrepareUI()
    {
        if (lockedHintUI != null)
        {
            lockedHintUI.SetActive(false);
        }

        if (fadePanel != null)
        {
            Color fadeColor = fadePanel.color;
            fadeColor.a = 0f;
            fadePanel.color = fadeColor;

            fadePanel.gameObject.SetActive(true);
        }

        if (youWinText != null)
        {
            Color textColor = youWinText.color;
            textColor.a = 0f;
            youWinText.color = textColor;

            youWinText.gameObject.SetActive(false);
        }
    }

    private void UpdateDoorGlow()
    {
        if (!glowDoor || doorGlow == null || isLoading)
        {
            return;
        }

        float pingPong = Mathf.PingPong(Time.time * glowSpeed, 1f);
        float alpha = Mathf.Lerp(minGlowAlpha, maxGlowAlpha, pingPong);

        Color color = doorGlow.color;
        color.a = alpha;
        doorGlow.color = color;
    }

    private void HandleBossDied()
    {
        bossIsDead = true;
        glowDoor = true;

        Debug.Log("Boss defeated! Exit door unlocked.");

        if (lockedHintUI != null)
        {
            lockedHintUI.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isLoading)
        {
            return;
        }

        bool canExit = !requireBossDefeated || bossIsDead;

        if (!canExit)
        {
            if (lockedHintUI != null)
            {
                lockedHintUI.SetActive(true);
            }

            return;
        }

        StartCoroutine(WinSequence());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (lockedHintUI != null)
        {
            lockedHintUI.SetActive(false);
        }
    }

    private IEnumerator WinSequence()
    {
        isLoading = true;
        glowDoor = false;

        if (lockedHintUI != null)
        {
            lockedHintUI.SetActive(false);
        }

        yield return FadeImage(fadePanel, 0f, 1f, fadeDuration);

        if (youWinText != null)
        {
            youWinText.gameObject.SetActive(true);
            yield return FadeText(youWinText, 0f, 1f, winTextDuration);
        }

        yield return new WaitForSecondsRealtime(delayBeforeLoad);

        if (GameManager.instance != null)
        {
            GameManager.instance.SavePlayerData();
        }

        LoadNextScene();
    }

    private IEnumerator FadeImage(
        Image image,
        float startAlpha,
        float endAlpha,
        float duration)
    {
        if (image == null)
        {
            yield break;
        }

        if (duration <= 0f)
        {
            SetImageAlpha(image, endAlpha);
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);
            float alpha = Mathf.Lerp(startAlpha, endAlpha, progress);

            SetImageAlpha(image, alpha);

            yield return null;
        }

        SetImageAlpha(image, endAlpha);
    }

    private IEnumerator FadeText(
        TMP_Text text,
        float startAlpha,
        float endAlpha,
        float duration)
    {
        if (text == null)
        {
            yield break;
        }

        if (duration <= 0f)
        {
            SetTextAlpha(text, endAlpha);
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);
            float alpha = Mathf.Lerp(startAlpha, endAlpha, progress);

            SetTextAlpha(text, alpha);

            yield return null;
        }

        SetTextAlpha(text, endAlpha);
    }

    private static void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private static void SetTextAlpha(TMP_Text text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }

    private void LoadNextScene()
    {
        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError(
                $"Next Scene Name chưa được nhập trên object {gameObject.name}."
            );

            isLoading = false;
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError(
                $"Scene '{nextSceneName}' chưa có trong Build Profiles/Build Settings."
            );

            isLoading = false;
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }
}