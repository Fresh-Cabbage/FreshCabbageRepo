using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float bounceStrength;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            collision.GetComponent<PlayerController>()?.BouncedOnTrampoline(bounceStrength);
        }
        else if (collision.CompareTag("Totem"))
        {
            GameObject other = collision.gameObject;

            Rigidbody2D rb2d = other.GetComponent<Rigidbody2D>();
            rb2d.velocity = new Vector2(rb2d.velocity.x, bounceStrength);
        }
        else if (collision.CompareTag("Hazard") && collision.GetComponent<Bullet>() != null) {
            collision.GetComponent<Bullet>().BouncedOnTrampoline(bounceStrength);
        }
    }


}
