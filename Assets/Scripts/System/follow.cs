using UnityEngine;

public class follow : MonoBehaviour
{
    [SerializeField] GameObject following;

    void Update()
    {
        if (following == null)
            return;

        transform.position =
            following.transform.position +
            new Vector3(0, 0, -10);
    }
}