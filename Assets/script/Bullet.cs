using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;

    [Header("Damage")]
    public int damage = 10;

    private Vector2 moveDirection;

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(
            moveDirection * speed * Time.deltaTime,
            Space.World
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit: " + collision.name);

        EnemyHealth enemy =
            collision.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            Debug.Log("Enemy Hit!");

            enemy.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
