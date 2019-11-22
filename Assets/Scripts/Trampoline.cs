using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public bool reactsToTotem;

    public EffectCheck effectCheck;
    Collider2D bounceHitbox;
    private bool active;

    public float bounceStrength;

    Animator anim;

    private void Start() {
        bounceHitbox = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    
        active = !reactsToTotem;
    }

    private void Update() {
        active = !reactsToTotem || effectCheck.IsColliding();

        bounceHitbox.enabled = active;
        anim.SetBool("Active", active);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!active)
            return;
        
        if (collision.CompareTag("Player")) {
            Bounced();
            collision.GetComponent<PlayerController>()?.BouncedOnTrampoline(bounceStrength);
        }
        else if (collision.CompareTag("Totem"))
        {
            Bounced();

            GameObject other = collision.gameObject;

            Rigidbody2D rb2d = other.GetComponent<Rigidbody2D>();
            rb2d.velocity = new Vector2(rb2d.velocity.x, bounceStrength);
        }
        else if (collision.CompareTag("Hazard") && collision.GetComponent<Bullet>() != null) {
            Bounced();
            collision.GetComponent<Bullet>().BouncedOnTrampoline(bounceStrength);
        }
    }


    private void Bounced() {
        anim.SetTrigger("Bounced");
    }
}
