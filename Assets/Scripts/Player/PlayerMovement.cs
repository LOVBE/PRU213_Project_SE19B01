using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float moveSpeedPerLevel = 0.25f;

    [Header("Shoot")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public int bulletDamage = 10;
    public int bulletDamagePerLevel = 2;

    [Header("Level")]
    public int playerLevel = 1;

    private float baseMoveSpeed;
    private int baseBulletDamage;
    private bool baseValuesSet = false;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private SpriteRenderer sr;

    [Header("Âm thanh Settings")]
    public AudioSource sfxSource;
    public AudioClip shootSound;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        baseMoveSpeed = moveSpeed;
        baseBulletDamage = bulletDamage;
        baseValuesSet = true;
    }

    void Start()
    {
        ApplyLevel();
    }

    void Update()
    {
        moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveInput.y = 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveInput.y = -1;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveInput.x = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveInput.x = 1;
        moveInput = moveInput.normalized;

        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("isMoving", isMoving);

        HandlePlayerFlip();

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
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        sr.flipX = mousePosition.x <= transform.position.x;
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 shootDirection = (mousePosition - firePoint.position).normalized;
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(shootDirection);
            bulletScript.damage = bulletDamage;
        }
        if (sfxSource != null && shootSound != null)
            sfxSource.PlayOneShot(shootSound);
    }

    public void LevelUp()
    {
        playerLevel++;
        ApplyLevel();
    }

    public void SetLevel(int level)
    {
        playerLevel = Mathf.Max(1, level);
        ApplyLevel();
    }

    void ApplyLevel()
    {
        // Nếu Awake() chưa chạy (chưa gán base values), bỏ qua
        // để tránh tính toán sai khi bị gọi quá sớm
        if (!baseValuesSet) return;

        int levelsGained = playerLevel - 1;
        moveSpeed = baseMoveSpeed + moveSpeedPerLevel * levelsGained;
        bulletDamage = baseBulletDamage + bulletDamagePerLevel * levelsGained;
        Debug.Log($"[PlayerMovement] ApplyLevel Lv{playerLevel}: moveSpeed={moveSpeed}, bulletDamage={bulletDamage}");
    }
}