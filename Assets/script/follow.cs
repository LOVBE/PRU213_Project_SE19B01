using UnityEngine;

public class follow : MonoBehaviour
{
    [SerializeField] GameObject following;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        transform.position = following.transform.position + new Vector3(0, 0, -10);
    }
}
