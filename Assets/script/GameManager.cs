using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Settings")]
    public GameObject gameOverPanel;

    [Header("Audio Clips")]
    public AudioClip mainGameBGM;
    public AudioClip gameOverBGM;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        BGM_Manager.Instance?.PlayBGM(mainGameBGM);
    }

    public void PlayerDied()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        BGM_Manager.Instance?.PlayBGM(gameOverBGM, loop: false);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        BGM_Manager.Instance?.SetLoop(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}