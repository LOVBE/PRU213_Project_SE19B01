using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillBarUI : MonoBehaviour
{
    [Header("Anchor")]
    public RectTransform anchor;

    [Header("Icons (tùy chọn)")]
    public Sprite iconLocked;
    public Sprite[] skillIcons = new Sprite[3];

    [Header("Visual")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color cooldownColor = new Color(0f, 0f, 0f, 0.6f);

    [Header("Layout")]
    public Vector2 iconSize = new Vector2(64f, 64f);
    public float spacing = 10f;
    public Vector2 startOffset = new Vector2(20f, -80f);

    private Image[] slotImages;
    private Image[] cooldownOverlays;
    private TMP_Text[] cooldownTexts;
    private TMP_Text[] keyHintTexts;
    private bool built = false;

    public void Build(SkillManager manager)
    {
        if (built) return;
        if (manager == null || manager.skills == null) return;

        RectTransform parent = anchor != null ? anchor : (transform as RectTransform);
        if (parent == null) return;

        slotImages = new Image[manager.skills.Length];
        cooldownOverlays = new Image[manager.skills.Length];
        cooldownTexts = new TMP_Text[manager.skills.Length];
        keyHintTexts = new TMP_Text[manager.skills.Length];

        for (int i = 0; i < manager.skills.Length; i++)
        {
            SkillDefinition def = manager.skills[i];

            GameObject slotGO = new GameObject("SkillSlot_" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            slotGO.transform.SetParent(parent, false);
            RectTransform slotRT = slotGO.GetComponent<RectTransform>();
            slotRT.anchorMin = new Vector2(0f, 1f);
            slotRT.anchorMax = new Vector2(0f, 1f);
            slotRT.pivot = new Vector2(0f, 1f);
            slotRT.sizeDelta = iconSize;
            slotRT.anchoredPosition = new Vector2(
                startOffset.x,
                startOffset.y - i * (iconSize.y + spacing)
            );

            Image bg = slotGO.GetComponent<Image>();
            bg.sprite = iconLocked != null ? iconLocked : null;
            bg.color = lockedColor;
            slotImages[i] = bg;

            GameObject overlayGO = new GameObject("CooldownOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            overlayGO.transform.SetParent(slotGO.transform, false);
            RectTransform overlayRT = overlayGO.GetComponent<RectTransform>();
            overlayRT.anchorMin = Vector2.zero;
            overlayRT.anchorMax = Vector2.one;
            overlayRT.offsetMin = Vector2.zero;
            overlayRT.offsetMax = Vector2.zero;
            overlayRT.pivot = new Vector2(0.5f, 1f);
            overlayRT.sizeDelta = new Vector2(0f, 0f);
            Image overlay = overlayGO.GetComponent<Image>();
            overlay.color = cooldownColor;
            overlay.raycastTarget = false;
            cooldownOverlays[i] = overlay;

            GameObject cooldownTextGO = new GameObject("CooldownText", typeof(RectTransform));
            cooldownTextGO.transform.SetParent(slotGO.transform, false);
            RectTransform ctRT = cooldownTextGO.GetComponent<RectTransform>();
            ctRT.anchorMin = Vector2.zero;
            ctRT.anchorMax = Vector2.one;
            ctRT.offsetMin = Vector2.zero;
            ctRT.offsetMax = Vector2.zero;
            TMP_Text ct = cooldownTextGO.AddComponent<TextMeshProUGUI>();
            ct.alignment = TextAlignmentOptions.Center;
            ct.fontSize = 28;
            ct.color = Color.white;
            ct.raycastTarget = false;
            ct.text = "";
            cooldownTexts[i] = ct;

            GameObject keyHintGO = new GameObject("KeyHint", typeof(RectTransform));
            keyHintGO.transform.SetParent(slotGO.transform, false);
            RectTransform khRT = keyHintGO.GetComponent<RectTransform>();
            khRT.anchorMin = new Vector2(0f, 0f);
            khRT.anchorMax = new Vector2(1f, 0f);
            khRT.pivot = new Vector2(0.5f, 0f);
            khRT.sizeDelta = new Vector2(0f, 18f);
            khRT.anchoredPosition = Vector2.zero;
            TMP_Text kh = keyHintGO.AddComponent<TextMeshProUGUI>();
            kh.alignment = TextAlignmentOptions.Center;
            kh.fontSize = 14;
            kh.color = Color.white;
            kh.raycastTarget = false;
            kh.text = KeyToString(def.key);
            keyHintTexts[i] = kh;
        }

        built = true;
    }

    public void Refresh(SkillManager manager)
    {
        if (!built)
        {
            Build(manager);
        }
        if (!built || manager == null) return;

        for (int i = 0; i < manager.skills.Length; i++)
        {
            bool unlocked = manager.IsUnlocked(i);
            float cdRemain = manager.GetCooldownRemaining(i);
            float cdMax = manager.GetCooldownMax(i);

            if (slotImages[i] != null)
            {
                if (unlocked && i < skillIcons.Length && skillIcons[i] != null)
                {
                    slotImages[i].sprite = skillIcons[i];
                }
                slotImages[i].color = unlocked ? unlockedColor : lockedColor;
            }

            float ratio = cdMax > 0f ? Mathf.Clamp01(cdRemain / cdMax) : 0f;

            if (cooldownOverlays[i] != null)
            {
                cooldownOverlays[i].gameObject.SetActive(ratio > 0f);
                cooldownOverlays[i].fillAmount = ratio;
                RectTransform rt = cooldownOverlays[i].rectTransform;
                rt.pivot = new Vector2(0.5f, 1f);
                rt.sizeDelta = new Vector2(0f, -rt.rect.height * ratio);
            }

            if (cooldownTexts[i] != null)
            {
                cooldownTexts[i].text = ratio > 0f ? Mathf.CeilToInt(cdRemain).ToString() : "";
            }
        }
    }

    string KeyToString(UnityEngine.InputSystem.Key key)
    {
        switch (key)
        {
            case UnityEngine.InputSystem.Key.Digit1: return "1";
            case UnityEngine.InputSystem.Key.Digit2: return "2";
            case UnityEngine.InputSystem.Key.Digit3: return "3";
            case UnityEngine.InputSystem.Key.Q: return "Q";
            case UnityEngine.InputSystem.Key.E: return "E";
            case UnityEngine.InputSystem.Key.R: return "R";
            default: return key.ToString();
        }
    }
}
