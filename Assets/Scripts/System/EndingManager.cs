using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class EndingManager : MonoBehaviour
{
    [Header("Ending Slides")]
    public Image storyImage;
    public Sprite[] slides;
    public TMP_Text storyText;
    public GameObject storyCharacter;
    public GameObject textFrame;

    [Header("Credits")]
    public GameObject creditsPanel;
    public GameObject centerCharacter;

    [Header("Ending Music")]
    public AudioClip endingBGM;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";

    private int currentSlide = 0;
    private bool storyStarted = false;
    private bool creditsShown = false;
    private bool canReturnToMenu = false;

    private readonly string[] texts =
    {
        "Miksi fought his way through Alpha Building, defeated the final monster, and finally escaped the nightmare alive.",

        "He immediately reported the horrifying truth to the police in Ngu Hanh Son... he meant, in Heidelberg."
    };

    private void Start()
    {
        Time.timeScale = 1f;

        currentSlide = 0;
        storyStarted = false;
        creditsShown = false;
        canReturnToMenu = false;

        if (storyCharacter != null)
            storyCharacter.SetActive(false);

        if (textFrame != null)
            textFrame.SetActive(false);

        if (storyText != null)
            storyText.text = "";

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        if (centerCharacter != null)
            centerCharacter.SetActive(false);

        if (storyImage != null && slides != null && slides.Length > 0)
            storyImage.sprite = slides[0];

        if (BGM_Manager.Instance != null && endingBGM != null)
        {
            BGM_Manager.Instance.PlayBGM(endingBGM, loop: true);
        }
    }

    private void Update()
    {
        if (creditsShown)
        {
            if (canReturnToMenu &&
                (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
            {
                ReturnToMainMenu();
            }

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!storyStarted)
            {
                StartStory();
            }
            else
            {
                NextSlide();
            }
        }
    }

    private void StartStory()
    {
        storyStarted = true;

        if (storyCharacter != null)
            storyCharacter.SetActive(true);

        if (textFrame != null)
            textFrame.SetActive(true);

        ShowSlide();
    }

    private void ShowSlide()
    {
        if (storyImage != null &&
            slides != null &&
            currentSlide >= 0 &&
            currentSlide < slides.Length)
        {
            storyImage.sprite = slides[currentSlide];
        }

        if (storyText != null &&
            currentSlide >= 0 &&
            currentSlide < texts.Length)
        {
            storyText.text = texts[currentSlide];
        }
    }

    private void NextSlide()
    {
        currentSlide++;

        int totalSlides = Mathf.Min(slides.Length, texts.Length);

        if (currentSlide >= totalSlides)
        {
            ShowCredits();
            return;
        }

        ShowSlide();
    }

    private void ShowCredits()
    {
        creditsShown = true;

        if (storyImage != null)
            storyImage.gameObject.SetActive(false);

        if (storyCharacter != null)
            storyCharacter.SetActive(false);

        if (textFrame != null)
            textFrame.SetActive(false);

        if (storyText != null)
            storyText.text = "";

        if (creditsPanel != null)
            creditsPanel.SetActive(true);

        if (centerCharacter != null)
            centerCharacter.SetActive(true);

        Invoke(nameof(EnableReturnToMenu), 1f);
    }

    private void EnableReturnToMenu()
    {
        canReturnToMenu = true;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}