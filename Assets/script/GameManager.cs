using UnityEngine;
using UnityEngine.SceneManagement; // BẮT BUỘC phải có dòng này để load lại Scene

public class GameManager : MonoBehaviour
{
    // Tạo cơ chế Singleton để các Script khác (như PlayerHealth) gọi GameManager cực nhanh
    public static GameManager instance;

    [Header("UI Settings")]
    public GameObject gameOverPanel; // Kéo cái GameOverPanel vào đây

    [Header("Audio Settings")]
    public AudioSource bgmSource;     // Kéo cái BGM_Manager vào đây
    public AudioClip mainGameBGM;     // Nhạc nền lúc chơi
    public AudioClip gameOverBGM;     // Nhạc nền lúc chết

    void Awake()
    {
        // Khởi tạo Singleton
        if (instance == null) instance = this;
    }

    void Start()
    {
        // 1. Đầu game phải ẩn UI chết đi
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // 2. Phát nhạc nền Main Game xập xình
        if (bgmSource != null && mainGameBGM != null)
        {
            bgmSource.clip = mainGameBGM;
            bgmSource.loop = true; // Nhạc nền chơi game thì phải lặp liên tục
            bgmSource.Play();
        }
    }

    // Hàm này sẽ được gọi khi thằng Player hết máu
    public void PlayerDied()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (bgmSource != null && gameOverBGM != null)
        {
            bgmSource.Stop();          // Tắt nhạc lúc đang quẩy
            bgmSource.clip = gameOverBGM;

            // DÒNG QUYẾT ĐỊNH ĐÂY: Ép nó không được loop!
            bgmSource.loop = false;

            bgmSource.Play();          // Bật nhạc đám ma 1 lần duy nhất
        }

        Time.timeScale = 0f;
    }

    // Hàm này gắn vào nút Restart để chơi lại
    public void RestartGame()
    {
        // CỰC KỲ CHÍ MẠNG: Phải rã băng thời gian về lại 1, nếu không load scene mới xong game sẽ bị đơ cứng!
        Time.timeScale = 1f;

        // Tự động lấy tên Scene hiện tại đang chơi và nạp lại từ đầu
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void LoadMainMenu()
    {
        // VẪN PHẢI RÃ BĂNG THỜI GIAN: Nếu không về Menu các hiệu ứng sẽ bị đơ
        Time.timeScale = 1f;

        // Load về màn hình Menu
        SceneManager.LoadScene("MainMenu");
    }
}