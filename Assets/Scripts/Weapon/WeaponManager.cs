using UnityEngine;
using System.Collections.Generic;
using System;

public class WeaponManager : MonoBehaviour
{
    [Header("Danh sách súng có trong game")]
    public List<WeaponData> allWeapons;

    [Header("Nơi hiển thị sprite súng trên nhân vật")]
    public SpriteRenderer weaponRenderer;

    [Header("Tham chiếu tới hệ thống level (kéo PlayerExperience vào đây)")]
    public PlayerExperience playerExperience;

    public WeaponData CurrentWeapon { get; private set; }

    // Bắn ra khi đổi súng, để UI hoặc script bắn khác lắng nghe nếu cần
    public event Action<WeaponData> OnWeaponChanged;

    void Start()
    {
        // Mặc định trang bị súng đầu tiên trong danh sách (AK)
        if (allWeapons.Count > 0)
            EquipWeapon(allWeapons[0]);
    }

    public bool IsUnlocked(WeaponData weapon)
    {
        int currentLevel = playerExperience != null ? playerExperience.currentLevel : 1;
        return currentLevel >= weapon.requiredLevel;
    }

    public bool IsEquipped(WeaponData weapon)
    {
        return CurrentWeapon == weapon;
    }

    public void EquipWeapon(WeaponData weapon)
    {
        if (weapon == null || !IsUnlocked(weapon)) return;

        CurrentWeapon = weapon;
        if (weaponRenderer != null)
            weaponRenderer.sprite = weapon.weaponSprite;

        OnWeaponChanged?.Invoke(weapon);
    }
}