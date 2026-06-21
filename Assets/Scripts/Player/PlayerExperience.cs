using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerExperience : MonoBehaviour
{
    public static PlayerExperience instance;

    [Header("Level System")]
    public int currentLevel = 1;
    public int currentExp = 0;

    public int[] expRequirements =
    {
        10,   // Lv1 -> Lv2
        20,   // Lv2 -> Lv3
        35,   // Lv3 -> Lv4
        55,   // Lv4 -> Lv5
        80,   // Lv5 -> Lv6
        110,  // Lv6 -> Lv7
        145,  // Lv7 -> Lv8
        185,  // Lv8 -> Lv9
        230,  // Lv9 -> Lv10
        280,  // Lv10 -> Lv11
        335,  // Lv11 -> Lv12
        395,  // Lv12 -> Lv13
        460,  // Lv13 -> Lv14
        530,  // Lv14 -> Lv15
        605,  // Lv15 -> Lv16
        685,  // Lv16 -> Lv17
        770,  // Lv17 -> Lv18
        860,  // Lv18 -> Lv19
        955    // Lv19 -> Lv20
    };
    [Header("UI")]
    public Slider expBar;
    public TMP_Text levelText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("HasSave", 0) == 1)
        {
            currentLevel =
                PlayerPrefs.GetInt("SavedLevel", 1);
            currentExp =
                PlayerPrefs.GetInt("SavedExp", 0);
        }

        UpdateUI();
    }
    public void AddExperience(int amount)
    {
        if (currentLevel >= 20)
            return;
        currentExp += amount;
        while (currentLevel < 20 &&
               currentExp >= expRequirements[currentLevel - 1])
        {
            currentExp -= expRequirements[currentLevel - 1];
            LevelUp();
        }
        UpdateUI();
    }
    void LevelUp()
    {
        currentLevel++;

        Debug.Log("LEVEL UP! Current Level: " + currentLevel);
        // Cập nhật UI Level
        if (levelText != null)
        {
            levelText.text = "Lv " + currentLevel;
        }
        // Sau này mở bảng nâng cấp ở đây
        // UpgradeManager.Instance.ShowUpgradePanel();
    }
    void UpdateUI()
    {
        if (levelText != null)
        {
            levelText.text = "Lv " + currentLevel;
        }
        if (expBar != null)
        {
            if (currentLevel >= 20)
            {
                expBar.maxValue = 1;
                expBar.value = 1;
            }
            else
            {
                expBar.maxValue =
                    expRequirements[currentLevel - 1];
                expBar.value = currentExp;
            }
        }
    }
    public void SaveData()
    {
        PlayerPrefs.SetInt("SavedLevel", currentLevel);
        PlayerPrefs.SetInt("SavedExp", currentExp);
    }
}