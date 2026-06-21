using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingButton : MonoBehaviour
{
    [Header("Panel")]
    public RectTransform settingPanel;
    public float slideSpeed = 800f;

    [Header("Settings")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider brightnessSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown languageDropdown;

    [Header("Back Button")]
    public GameObject backButton;

    private Vector2 openPos;
    private Vector2 closePos;
    private bool isOpen = false;
    private bool isAnimating = false;

    void Start()
    {
        float panelWidth = settingPanel.rect.width;
        openPos = new Vector2(0, 0);
        closePos = new Vector2(panelWidth + 50, 0);
        settingPanel.anchoredPosition = closePos;
        backButton.SetActive(false);

        // Load giá trị đã lưu vào slider
        musicSlider.value = PlayerPrefs.GetFloat("Music", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFX", 1f);
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        languageDropdown.value = PlayerPrefs.GetInt("Language", 0);

        // ✅ Slider âm thanh → BGM_Manager → AudioMixer (hiệu lực toàn game)
        musicSlider.onValueChanged.AddListener(v =>
            BGM_Manager.Instance?.SetBGMVolume(v));

        sfxSlider.onValueChanged.AddListener(v =>
            BGM_Manager.Instance?.SetSFXVolume(v));

        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        languageDropdown.onValueChanged.AddListener(v =>
            PlayerPrefs.SetInt("Language", v));
    }

    void Update()
    {
        if (!isAnimating) return;
        Vector2 target = isOpen ? openPos : closePos;
        settingPanel.anchoredPosition = Vector2.MoveTowards(
            settingPanel.anchoredPosition, target, slideSpeed * Time.deltaTime);
        if (Vector2.Distance(settingPanel.anchoredPosition, target) < 1f)
        {
            settingPanel.anchoredPosition = target;
            isAnimating = false;
        }
    }

    public void ToggleSetting()
    {
        isOpen = !isOpen;
        isAnimating = true;
        backButton.SetActive(isOpen);
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
        Image overlayImg = overlay.GetComponent<Image>();
        overlayImg.color = new Color(0, 0, 0, 1f - value) * 0.2f;
    }

    void OnFullscreenChanged(bool value)
    {
        Screen.fullScreen = value;
        if (value)
            Screen.SetResolution(Screen.currentResolution.width,
                                 Screen.currentResolution.height, true);
        else
            Screen.SetResolution(1280, 720, false);
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }
}