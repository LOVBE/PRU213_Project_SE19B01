using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class IntroManage : MonoBehaviour
{
    public Image storyImage;
    public Sprite[] slides;
    public TMP_Text storyText;
    public GameObject character;
    public GameObject textFrame;

    [Header("Back Button")]
    public GameObject backButton; 
    private int currentSlide = 0;
    private bool started = false;

    string[] texts =
    {
        "Heidelberg, Germany. Third-year student Adolf Miksi stares at his tuition invoice — 1,400 USD per semester. Something was very wrong.",
        "Determined to find the truth, he slipped into Alpha Building late at night — a place always full of technicians, professors, and suspicious strangers.",
        "He climbed to the fifth floor. At the center of the building, a strange locked room caught his eye.",
        "He pushed open the door... and froze. A secret monster research facility — hidden deep inside his own university.",
        "Before he could run, he knocked something over. The alarm screamed through the halls.",
        "The monsters had heard him. Armed with whatever weapons he could find, Miksi had one choice — fight his way out.",
        "He had to escape Alpha Building and expose the truth to the police in Heidelberg. The hunt had begun."
    };

    void Start()
    {
        character.SetActive(false);
        textFrame.SetActive(false);
        storyText.text = "";

        
        if (backButton != null)
            backButton.SetActive(false);

        if (slides.Length > 0)
            storyImage.sprite = slides[0];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (!started)
            {
                started = true;

                character.SetActive(true);
                textFrame.SetActive(true);

                if (backButton != null)
                    backButton.SetActive(true);

                ShowSlide();
            }
            else
            {
                NextSlide();
            }
        }
    }

    public void GoBackToMenu()
    {
        SceneManager.LoadScene("Mainmenu");
    }

    void ShowSlide()
    {
        if (currentSlide < slides.Length)
            storyImage.sprite = slides[currentSlide];
        if (currentSlide < texts.Length)
            storyText.text = texts[currentSlide];
    }

    void NextSlide()
    {
        currentSlide++;
        if (currentSlide >= slides.Length)
        {
            SceneManager.LoadScene("MainGame");
            return;
        }
        ShowSlide();
    }
}