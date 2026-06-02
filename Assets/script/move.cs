using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private Animator animator;
    private SpriteRenderer sr;

    [Header("Shoot")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
        {
            moveInput.y = 1;
        }

        if (Keyboard.current.sKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed)
        {
            moveInput.y = -1;
        }

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            moveInput.x = -1;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            moveInput.x = 1;
        }

        // Chuẩn hóa vector
        moveInput = moveInput.normalized;

        // Animation
        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("isMoving", isMoving);

        // Flip nhân vật
        if (moveInput.x > 0)
        {
            sr.flipX = false;
        }
        else if (moveInput.x < 0)
        {
            sr.flipX = true;
        }

        // Bắn
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        // Di chuyển bằng Rigidbody2D
        rb.MovePosition(
            rb.position +
            moveInput * moveSpeed * Time.fixedDeltaTime
        );
    }

    void Shoot()
    {
        // Lấy vị trí chuột trong world
        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(
                Mouse.current.position.ReadValue()
            );

        // Hướng bắn
        Vector2 direction =
            (mousePosition - firePoint.position);

        // Tạo đạn
        GameObject bullet =
            Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.identity
            );

        // Xoay đạn
        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        bullet.transform.rotation =
            Quaternion.Euler(0, 0, angle);

        // Truyền hướng
        bullet.GetComponent<Bullet>()
              .SetDirection(direction);
    }
}