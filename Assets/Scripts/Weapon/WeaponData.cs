using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string weaponName;
    public Sprite icon;              // icon hiển thị trong panel chọn súng
    public Sprite weaponSprite;      // sprite gắn lên tay nhân vật

    [Header("Yêu cầu mở khoá")]
    public int requiredLevel = 1;    // level 1 = mặc định, level 4 = súng mới

    [Header("Chỉ số (tuỳ chọn, nếu súng khác damage/tốc độ bắn)")]
    public int damage = 10;
    public float fireRate = 0.2f;
}