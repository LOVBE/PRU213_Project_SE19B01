using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public AudioClip menuBGM;
    public GameObject continueButton; // kéo nút Continue vào đây

    void Start()
    {
        Debug.Log("MainMenuManager Start, Instance = " + BGM_Manager.Instance);
        Debug.Log("menuBGM = " + menuBGM);
        BGM_Manager.Instance?.PlayBGM(menuBGM);

        // Chỉ hiện Continue nếu có save
        if (continueButton != null)
            continueButton.SetActive(PlayerPrefs.GetInt("HasSave", 0) == 1);
    }

    public void PlayGame()
    {
        // Xóa save cũ khi bấm Start mới
        PlayerPrefs.DeleteKey("HasSave");
        PlayerPrefs.DeleteKey("SavedHP");
        PlayerPrefs.DeleteKey("LastScene");
        PlayerPrefs.Save();

        SceneManager.LoadScene("Intro");
    }

    public void ContinueGame()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", "MainGame");
        SceneManager.LoadScene(lastScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}