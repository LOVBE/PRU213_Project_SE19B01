using UnityEngine;

public class TeddyBear : MonoBehaviour
{
    #region Fields/Attributes
    Rigidbody2D rb2d;
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Vector2 force = new Vector2(2f, 0f);
        rb2d.AddForce(force, ForceMode2D.Impulse);
    }
}
