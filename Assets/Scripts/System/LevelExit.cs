using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelExitTrigger : MonoBehaviour
{

    [Header("UI")]
    public GameObject lockedHintUI;
    public Image fadePanel;
    public TMP_Text youWinText;

    [Header("Door Glow")]
    public SpriteRenderer doorGlow;

    private bool bossIsDead = false;
    private bool isLoading = false;
    private bool glowDoor = false;

    void OnEnable()
    {
        EnemyHealth.OnBossDied += HandleBossDied;
    }

    void OnDisable()
    {
        EnemyHealth.OnBossDied -= HandleBossDied;
    }

    void Update()
    {

        if (glowDoor && doorGlow != null)
        {
            float alpha = Mathf.Lerp(0.3f, 1f,
                Mathf.PingPong(Time.time * 2f, 1));

            Color c = doorGlow.color;
            c.a = alpha;
            doorGlow.color = c;
        }
    }

    void HandleBossDied()
    {
        bossIsDead = true;

        glowDoor = true;

        Debug.Log("Boss defeated!");

        if (lockedHintUI != null)
            lockedHintUI.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!bossIsDead)
        {
            if (lockedHintUI != null)
                lockedHintUI.SetActive(true);

            return;
        }

        if (!isLoading)
        {
            StartCoroutine(WinSequence());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (lockedHintUI != null)
                lockedHintUI.SetActive(false);
        }
    }

    IEnumerator WinSequence()
    {
        isLoading = true;

        glowDoor = false;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;

            Color c = fadePanel.color;
            c.a = t;

            fadePanel.color = c;

            yield return null;
        }


        youWinText.gameObject.SetActive(true);

        t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;

            Color c = youWinText.color;
            c.a = t;

            youWinText.color = c;

            yield return null;
        }

        yield return new WaitForSeconds(2);

        GameManager.instance?.SavePlayerData();

        SceneManager.LoadScene("IntroLevel3");
    }
}