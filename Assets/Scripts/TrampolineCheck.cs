using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineCheck : MonoBehaviour
{
    public float bounceStrength = 5f;

    // Start is called before the first frame update
    void Start()
    {
     
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb;
        if (collision.CompareTag("Player") || collision.CompareTag("Totem"))
        {
            GameObject other = collision.gameObject;

            rb = other.GetComponent<Rigidbody2D>();

            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + bounceStrength);
        }
    }
} //Fun fact: Unity makes me angy.
