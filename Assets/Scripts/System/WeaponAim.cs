using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAim : MonoBehaviour
{
    private Camera mainCam;
    private SpriteRenderer gunSprite;

    void Start()
    {
        mainCam = Camera.main;
        gunSprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Vector3 lookDir = mousePos - transform.position;

        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);

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