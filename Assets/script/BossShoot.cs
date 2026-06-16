using UnityEngine;

public class BossShoot : MonoBehaviour
{
    [Header("References")]
    public GameObject bamiaBulletPrefab; 
    public Transform firePoint;        

    [Header("Shoot Settings")]
    public float fireRate = 2f;          

    private float timer;
    private Transform player;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject target = GameObject.FindGameObjectWithTag("Player");

        if (target != null)
        {
            player = target.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            animator.SetTrigger("Shoot");

            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
        if (bamiaBulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Ê ông ơi, quên kéo đạn hoặc nòng súng vào script BossShoot rồi!");
            return;
        }

        GameObject bulletObj = Instantiate(bamiaBulletPrefab, firePoint.position, Quaternion.identity);

      
        Vector2 targetDirection = (player.position - firePoint.position).normalized;

       
        BossBullet bulletScript = bulletObj.GetComponent<BossBullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(targetDirection);
        }
    }
}