using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InGameSettingButton : MonoBehaviour
{
    [Header("Panel")]
    public GameObject settingPanel;
    public GameObject confirmRetryPanel; // popup xác nhận retry

    [Header("Settings Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider brightnessSlider;

    private bool isOpen = false;

    void Start()
    {
        settingPanel.SetActive(false);
        confirmRetryPanel.SetActive(false); // ẩn popup lúc đầu

        musicSlider.value = PlayerPrefs.GetFloat("Music", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1f);
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);

        musicSlider.onValueChanged.AddListener(v =>
            BGM_Manager.Instance?.SetBGMVolume(v));
        sfxSlider.onValueChanged.AddListener(v =>
            BGM_Manager.Instance?.SetSFXVolume(v));
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
    }

    public void ToggleSetting()
    {
        isOpen = !isOpen;
        settingPanel.SetActive(isOpen);
        Time.timeScale = isOpen ? 0f : 1f;
    }

    public void ResumeGame()
    {
        isOpen = false;
        settingPanel.SetActive(false);
        confirmRetryPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnRetryClicked()
    {
        confirmRetryPanel.SetActive(true);
    }

    public void ConfirmRetry()
    {
        confirmRetryPanel.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CancelRetry()
    {
        confirmRetryPanel.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        SaveProgress();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void SaveProgress()
    {
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);

        if (PlayerHealth.instance != null)
            PlayerPrefs.SetInt("SavedHP", PlayerHealth.instance.currentHealth);

        PlayerPrefs.SetInt("HasSave", 1);
        PlayerPrefs.Save();
        Debug.Log("Đã lưu tiến trình! HP = " + PlayerHealth.instance?.currentHealth);
    }

    void OnBrightnessChanged(float value)
    {
        PlayerPrefs.SetFloat("Brightness", value);
        Canvas canvas = FindObjectOfType<Canvas>();
        Transform overlay = canvas.transform.Find("BrightnessOverlay");
        if (overlay == null)
        {
            GameObject obj = new GameObject("BrightnessOverlay");
            obj.transform.SetParent(canvas.transform, false);
            RectTransform rt = obj.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            Image img = obj.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0);
            img.raycastTarget = false;
            overlay = obj.transform;
        }
        overlay.GetComponent<Image>().color = new Color(0, 0, 0, 1f - value) * 0.2f;
    }
}