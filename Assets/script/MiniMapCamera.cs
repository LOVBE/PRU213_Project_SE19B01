using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform player; // kéo Player vào

    void LateUpdate()
    {
        if (player == null) return;
        // Chỉ follow X Y, giữ Z cố định
        transform.position = new Vector3(
            player.position.x,
            player.position.y,
            transform.position.z
        );
    }
}