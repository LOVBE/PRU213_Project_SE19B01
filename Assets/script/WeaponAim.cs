using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAim : MonoBehaviour
{
    private Camera mainCam;
    private SpriteRenderer gunSprite;

    void Start()
    {
        mainCam = Camera.main;
        // Lấy SpriteRenderer của khẩu AK (nằm ở Object con)
        gunSprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // 1. Lấy vị trí chuột bằng New Input System giống player
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // 2. Tính hướng từ tâm súng tới chuột
        Vector3 lookDir = mousePos - transform.position;

        // 3. Tính góc xoay trục Z
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        // 4. Xoay GunPivot
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 5. Lật khẩu súng lên/xuống để súng không bị chổng ngược báng khi quay sang trái
        if (gunSprite != null)
        {
            if (angle > 90 || angle < -90)
            {
                gunSprite.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
            }
            else
            {
                gunSprite.transform.localRotation = Quaternion.identity;
            }
        }
    }
}