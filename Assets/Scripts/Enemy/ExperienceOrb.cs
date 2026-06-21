using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    public int expValue = 10;
    public float pickupRange = 3f;
    public float moveSpeed = 6f;

    private Transform player;

    private void Start()
    {
        player = GameObject
            .FindGameObjectWithTag("Player")
            ?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.position);

        if (distance <= pickupRange)
        {
            transform.position =
                Vector2.MoveTowards(
                    transform.position,
                    player.position,
                    moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerExperience exp =
                other.GetComponent<PlayerExperience>();

            if (exp != null)
            {
                exp.AddExperience(expValue);
            }

            Destroy(gameObject);
        }
    }
}