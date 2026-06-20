using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class IntroLevel3 : MonoBehaviour
{
    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public TMP_Text continueText;

    [Header("Portrait")]
    public Image leftPortrait;
    public Image rightPortrait;

    public Sprite miksiSprite;
    public Sprite twanSprite;

    [Header("Scene Settings")]
    [Tooltip("Tên scene gameplay sẽ load sau khi dialogue kết thúc. Phải khớp tên trong Build Settings")]
    public string nextSceneName = "Level3";

    [Header("Settings")]
    public float startDelay = 2f;
    public float typingSpeed = 0.03f;

    private int index = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    string[] speakers =
    {
        "",
        "Twan Viem",
        "Adolf Miksi",
        "Twan Viem",
        "Adolf Miksi",
        "Twan Viem"
    };

    string[] dialogues =
    {
        "",
        "Well, well... Have you passed LAB211 yet?",
        "So... you're the one behind all of this?",
        "Not exactly. Students rely too much on AI these days. I simply made a little extra money by conducting a few 'small' experiments.",
        "Those aren't experiments! Too many innocent people have already died because of you.",
        "<color=#ff4040><b>Then... try to stop me!</b></color>"
    };

    void Start()
    {
        // Ẩn UI lúc mới vào scene
        dialoguePanel.SetActive(false);
        continueText.gameObject.SetActive(false);

        // Đợi 2 giây rồi hiện dialogue
        StartCoroutine(StartDialogueAfterDelay());
    }

    IEnumerator StartDialogueAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);
        StartDialogue();
    }

    void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        index = 1;
        ShowDialogue();
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // Nếu đang gõ chữ thì hiện hết luôn
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogues[index];
                isTyping = false;
                continueText.gameObject.SetActive(true);
            }
            else
            {
                NextDialogue();
            }
        }
    }

    void ShowDialogue()
    {
        nameText.text = speakers[index];
        continueText.gameObject.SetActive(false);

        // Đổi Portrait
        leftPortrait.sprite = miksiSprite;
        rightPortrait.sprite = twanSprite;

        Color bright = Color.white;
        Color dark = new Color(1f, 1f, 1f, 0.35f);

        if (speakers[index] == "Adolf Miksi")
        {
            leftPortrait.color = bright;
            rightPortrait.color = dark;
        }
        else
        {
            rightPortrait.color = bright;
            leftPortrait.color = dark;
        }

        // Hiệu ứng gõ chữ
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(dialogues[index]));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in sentence)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        continueText.gameObject.SetActive(true);
    }

    void NextDialogue()
    {
        index++;

        if (index >= dialogues.Length)
        {
            EndDialogue();
            return;
        }

        ShowDialogue();
    }

    void EndDialogue()
    {
        // Ẩn Dialogue
        dialoguePanel.SetActive(false);

        // Chuyển sang scene gameplay chính
        Debug.Log("[Level3Manage] Dialogue kết thúc -> Load scene: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}