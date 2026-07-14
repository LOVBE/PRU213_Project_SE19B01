using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSlotUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public GameObject lockIcon;
    public TMP_Text requiredLevelText;
    public Button selectButton;

    private WeaponData data;
    private WeaponManager manager;
    private WeaponSelectionPanel panel;

    public void Setup(
        WeaponData weaponData,
        WeaponManager weaponManager,
        WeaponSelectionPanel selectionPanel)
    {
        data = weaponData;
        manager = weaponManager;
        panel = selectionPanel;

        iconImage.sprite = data.icon;
        nameText.text = data.weaponName;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnClick);

        RefreshState();
    }

    public void RefreshState()
    {
        bool unlocked = manager.IsUnlocked(data);
        bool isEquipped = manager.IsEquipped(data);

        if (isEquipped)
        {
            // Đang dùng -> hiển thị rõ
            iconImage.color = Color.white;
            nameText.color = Color.white;
            requiredLevelText.color = Color.white;

            lockIcon.SetActive(false);
            requiredLevelText.gameObject.SetActive(true);
            requiredLevelText.text = "Đang sử dụng";

            selectButton.interactable = true;
        }
        else if (!unlocked)
        {
            // Chưa mở khóa -> làm mờ
            iconImage.color = new Color(1f, 1f, 1f, 0.4f);
            nameText.color = new Color(1f, 1f, 1f, 0.6f);
            requiredLevelText.color = new Color(1f, 1f, 1f, 0.8f);

            lockIcon.SetActive(true);
            requiredLevelText.gameObject.SetActive(true);
            requiredLevelText.text = $"Require Lv.{data.requiredLevel}";

            selectButton.interactable = false;
        }
        else
        {
            // Đã mở khóa nhưng chưa dùng -> hiển thị rõ
            iconImage.color = Color.white;
            nameText.color = Color.white;
            requiredLevelText.gameObject.SetActive(false);

            lockIcon.SetActive(false);
            selectButton.interactable = true;
        }
    }

    private void OnClick()
    {
        if (!manager.IsEquipped(data))
        {
            manager.EquipWeapon(data);
        }

        panel.OnWeaponSelected();
    }
}