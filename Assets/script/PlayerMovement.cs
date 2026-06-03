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
    // firePoint này tí nữa ông kéo Object "Muzzle" (đầu nòng) của khẩu AK vào nhé
    public Transform firePoint;

    [Header("Âm thanh Settings")]
    public AudioSource sfxSource;     // Kéo cái Loa vừa tạo vào đây
    public AudioClip shootSound;      // Kéo file mp3/wav tiếng súng vào đây

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveInput.y = 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveInput.y = -1;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput.x = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveInput.x = 1;

        moveInput = moveInput.normalized;

        // Animation
        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("isMoving", isMoving);

        // --- Xử lý Lật (Flip) Player theo hướng chuột thay vì hướng đi ---
        HandlePlayerFlip();

        // Bắn
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void HandlePlayerFlip()
    {
        // Lấy vị trí chuột để biết Player đang nhìn về bên trái hay bên phải
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (mousePosition.x > transform.position.x)
        {
            sr.flipX = false; // Chuột bên phải -> Nhìn bên phải
        }
        else
        {
            sr.flipX = true;  // Chuột bên trái -> Nhìn bên trái
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // 1. Tạo viên đạn ngay tại vị trí nòng súng (firePoint)
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 2. Lấy vị trí chuột trong không gian Game (World Space) bằng New Input System
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // 3. Tính toán hướng chuẩn xác: Lấy vị trí chuột trừ đi vị trí nòng súng
        Vector2 shootDirection = (mousePosition - firePoint.position).normalized;

        // 4. Gọi script Bullet để truyền hướng bay chính xác cho viên đạn
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(shootDirection);
        }

        // 2. Logic phát âm thanh (Thêm đoạn này vào)
        if (sfxSource != null && shootSound != null)
        {
            // Dùng PlayOneShot để sấy AK đạn ra liên tục tiếng không bị ngắt quãng
            sfxSource.PlayOneShot(shootSound);
        }
    }
}