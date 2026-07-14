using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponSelectionPanel : MonoBehaviour
{
    [Header("Tham chiếu")]
    public WeaponManager weaponManager;
    public GameObject panelRoot;
    public Transform listContainer;
    public GameObject weaponSlotPrefab;

    bool hasPopulated = false;

    void Start()
    {
        panelRoot.SetActive(false);
        // KHÔNG gọi PopulateList() ở đây nữa, vì panel đang tắt
    }

    public void TogglePanel()
    {
        bool willShow = !panelRoot.activeSelf;
        panelRoot.SetActive(willShow);

        Time.timeScale = willShow ? 0f : 1f;

        if (willShow)
        {
            if (!hasPopulated)
            {
                PopulateList();
                hasPopulated = true;
            }
            else
            {
                RefreshLockStates();
            }
            StartCoroutine(RebuildLayoutNextFrame());
        }
    }

    void PopulateList()
    {
        foreach (Transform child in listContainer)
            Destroy(child.gameObject);

        foreach (var weapon in weaponManager.allWeapons)
        {
            GameObject slot = Instantiate(weaponSlotPrefab, listContainer);
            WeaponSlotUI slotUI = slot.GetComponent<WeaponSlotUI>();
            if (slotUI == null)
            {
                Debug.LogError("WeaponSlotUI component không tìm thấy trên prefab!");
                continue;
            }
            slotUI.Setup(weapon, weaponManager, this);
        }
    }

    void RefreshLockStates()
    {
        foreach (Transform child in listContainer)
        {
            WeaponSlotUI slotUI = child.GetComponent<WeaponSlotUI>();
            if (slotUI != null)
                slotUI.RefreshState();
        }
    }

    // Đợi 1 frame sau khi panel active rồi mới ép rebuild — đảm bảo chắc chắn ăn
    IEnumerator RebuildLayoutNextFrame()
    {
        yield return null; // đợi 1 frame
        RectTransform contentRect = listContainer.GetComponent<RectTransform>();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
    }

    public void OnWeaponSelected()
    {
        panelRoot.SetActive(false);
        Time.timeScale = 1f;
    }
}