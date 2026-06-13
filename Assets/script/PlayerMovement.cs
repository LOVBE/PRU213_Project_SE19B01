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

    [Header("Âm thanh Settings")]
    public AudioSource sfxSource;   
    public AudioClip shootSound;      

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

        if (mousePosition.x > transform.position.x)
        {
            sr.flipX = false; 
        }
        else
        {
            sr.flipX = true; 
        }
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
        }

        if (sfxSource != null && shootSound != null)
        {
            sfxSource.PlayOneShot(shootSound);
        }
    }
}